using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PlutoPoint_Launcher.Services;

public sealed record InstallerRelease(string Tag, Version Version, int Beta, Uri DownloadUrl)
{
    public bool IsBeta => Beta > 0;
}

public sealed class ReleaseService
{
    private const string ReleasesUrl =
        "https://api.github.com/repos/ProfessorShroom/PlutoPoint-Installer/releases?per_page=50";

    private const string AssetName = "Computer-Repair-Centre-Installer.exe";

    private static readonly Regex TagRegex = new(
        @"^v(\d+\.\d+\.\d+\.\d+)(?:b(\d+))?$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly HttpClient _http;
    private readonly bool _includeBetas;

    public ReleaseService(HttpClient http, bool includeBetas = true)
    {
        _http = http;
        _includeBetas = includeBetas;
    }

    public async Task<InstallerRelease?> GetNewestAsync(CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ReleasesUrl);
        request.Headers.Accept.ParseAdd("application/vnd.github+json");

        using var response = await _http.SendAsync(request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct).ConfigureAwait(false);

        InstallerRelease? newest = null;

        foreach (var release in document.RootElement.EnumerateArray())
        {
            if (release.TryGetProperty("draft", out var draft) && draft.ValueKind == JsonValueKind.True)
                continue;

            var candidate = Parse(release);
            if (candidate is null || (candidate.IsBeta && !_includeBetas))
                continue;

            if (newest is null || IsNewer(candidate, newest))
                newest = candidate;
        }

        return newest;
    }

    private static InstallerRelease? Parse(JsonElement release)
    {
        if (!release.TryGetProperty("tag_name", out var tagElement))
            return null;

        var tag = tagElement.GetString();
        if (string.IsNullOrWhiteSpace(tag))
            return null;

        var match = TagRegex.Match(tag);
        if (!match.Success || !Version.TryParse(match.Groups[1].Value, out var version))
            return null;

        var beta = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;

        if (!release.TryGetProperty("assets", out var assets) || assets.ValueKind != JsonValueKind.Array)
            return null;

        foreach (var asset in assets.EnumerateArray())
        {
            var name = asset.TryGetProperty("name", out var n) ? n.GetString() : null;
            if (!string.Equals(name, AssetName, StringComparison.OrdinalIgnoreCase))
                continue;

            var url = asset.TryGetProperty("browser_download_url", out var u) ? u.GetString() : null;
            if (Uri.TryCreate(url, UriKind.Absolute, out var downloadUrl))
                return new InstallerRelease(tag, version, beta, downloadUrl);
        }

        return null;
    }

    public static bool IsNewer(InstallerRelease candidate, InstallerRelease current)
    {
        var byVersion = candidate.Version.CompareTo(current.Version);
        if (byVersion != 0)
            return byVersion > 0;

        if (candidate.IsBeta != current.IsBeta)
            return !candidate.IsBeta;

        return candidate.Beta > current.Beta;
    }
}
