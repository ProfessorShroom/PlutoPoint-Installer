using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public sealed class Downloader
    {
        private static readonly TimeSpan StallTimeout = TimeSpan.FromSeconds(60);

        public static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };
        public static readonly HttpClient BrowserHttp = CreateBrowserClient();

        private readonly Action<string> _log;

        public Downloader(Action<string> log) => _log = log;

        private static HttpClient CreateBrowserClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                AllowAutoRedirect = true
            };
            var client = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(15) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-GB,en;q=0.9");
            return client;
        }

        public async Task<bool> DownloadWithRetryAsync(Uri url, string path, string label, CancellationToken stop, HttpClient? client = null, int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    using HttpResponseMessage response = await (client ?? Http).GetAsync(url, HttpCompletionOption.ResponseHeadersRead, stop);
                    response.EnsureSuccessStatusCode();
                    await SaveAsync(response, path, stop);
                    return true;
                }
                catch (Exception ex)
                {
                    TryDelete(path);
                    stop.ThrowIfCancellationRequested();
                    if (attempt == maxAttempts)
                    {
                        _log($"❌ Failed to download {label}: {ex.Message}");
                        return false;
                    }
                    _log($"⚠️ Download failed (attempt {attempt}/{maxAttempts}): {ex.Message}. Retrying...");
                    await Task.Delay(1500 * attempt, stop);
                }
            }
            return false;
        }

        public async Task<bool> DownloadInstallerAsync(HttpClient client, string url, string path, string? referer, CancellationToken stop)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (referer != null) request.Headers.Referrer = new Uri(referer);
            request.Headers.Accept.Clear();
            request.Headers.Accept.ParseAdd("*/*");
            using HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, stop);
            if (!response.IsSuccessStatusCode)
            {
                _log($"⚠️ {url} returned {(int)response.StatusCode}.");
                return false;
            }
            string mediaType = response.Content.Headers.ContentType?.MediaType ?? "";
            if (mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
            {
                _log($"⚠️ {url} returned a web page instead of an installer.");
                return false;
            }
            try
            {
                await SaveAsync(response, path, stop);
            }
            catch
            {
                TryDelete(path);
                throw;
            }
            if (new FileInfo(path).Length < 1024 * 1024)
            {
                _log($"⚠️ {url} downloaded a file that is too small to be the installer.");
                return false;
            }
            return true;
        }

        private static async Task SaveAsync(HttpResponseMessage response, string path, CancellationToken stop)
        {
            await using Stream source = await response.Content.ReadAsStreamAsync(stop);
            await using var target = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            using var stall = CancellationTokenSource.CreateLinkedTokenSource(stop);
            var buffer = new byte[81920];
            try
            {
                while (true)
                {
                    stall.CancelAfter(StallTimeout);
                    int read = await source.ReadAsync(buffer, stall.Token);
                    if (read == 0)
                        return;
                    await target.WriteAsync(buffer.AsMemory(0, read), stop);
                }
            }
            catch (OperationCanceledException) when (!stop.IsCancellationRequested)
            {
                throw new TimeoutException($"no data was received for {StallTimeout.TotalSeconds:0} seconds");
            }
        }

        private static void TryDelete(string path)
        {
            try { File.Delete(path); } catch { }
        }
    }
}
