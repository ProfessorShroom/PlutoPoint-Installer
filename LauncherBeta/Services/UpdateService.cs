using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PlutoPoint_Launcher.Services;

public sealed record UpdateInfo(
    bool IsAvailable,
    Version? Version,
    Uri? DownloadUrl,
    string? Changelog,
    bool Mandatory);

public sealed class UpdateService
{
    private const string FeedUrl =
        "https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/updateBeta.xml";

    private readonly HttpClient _http;

    public UpdateService(HttpClient http) => _http = http;

    public async Task<UpdateInfo> CheckForUpdateAsync(Version currentVersion, CancellationToken ct = default)
    {
        var xml = await _http.GetStringAsync(FeedUrl, ct).ConfigureAwait(false);
        var item = XDocument.Parse(xml).Root;

        if (item is null)
            return new UpdateInfo(false, null, null, null, false);

        var remoteVersion = Version.Parse(item.Element("version")?.Value ?? "0.0.0.0");
        var urlText = item.Element("url")?.Value;
        var url = string.IsNullOrWhiteSpace(urlText) ? null : new Uri(urlText);
        var changelog = item.Element("changelog")?.Value;
        var mandatory = bool.TryParse(item.Element("mandatory")?.Value, out var m) && m;

        var isNewer = url is not null && remoteVersion > currentVersion;
        return new UpdateInfo(isNewer, remoteVersion, url, changelog, mandatory);
    }
}
