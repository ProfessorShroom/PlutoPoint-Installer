using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PlutoPoint_Installer.Models;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public static class RemoteData
    {
        private const string BaseUrl = "https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/Data/";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private static readonly string[] IpServices =
        {
            "https://api.ipify.org",
            "https://ipv4.icanhazip.com",
            "https://ifconfig.me/ip",
            "https://checkip.amazonaws.com"
        };

        public static Task<DownloadUrls?> GetDownloadUrlsAsync(Action<string> log) => LoadAsync<DownloadUrls>("Downloads.json", "URLs", log);

        public static Task<PasswordHashes?> GetPasswordHashesAsync(Action<string> log) => LoadAsync<PasswordHashes>("Passwords.json", "hashes", log);

        public static async Task<Shop> DetectShopAsync(Action<string> log)
        {
            string? ip = await GetPublicIpAsync();
            if (ip == null)
            {
                log("⚠️ Couldn't get this computer's public IP address.");
                return Shop.Unknown;
            }
            LocationHashes? hashes = await LoadAsync<LocationHashes>("IPs.json", "hashes", log);
            return hashes == null ? Shop.Unknown : MatchShop(Sha256(ip), hashes);
        }

        public static Shop MatchShop(string ipHash, LocationHashes hashes)
        {
            if (ipHash == hashes.romsey) return Shop.Romsey;
            if (ipHash == hashes.chandlersFord) return Shop.ChandlersFord;
            if (ipHash == hashes.highcliffe) return Shop.Highcliffe;
            return Shop.Unknown;
        }

        public static string Sha256(string text) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();

        private static async Task<T?> LoadAsync<T>(string file, string what, Action<string> log) where T : class
        {
            try
            {
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                string json = await Downloader.Http.GetStringAsync(BaseUrl + file, timeout.Token);
                return JsonSerializer.Deserialize<T>(json, JsonOptions);
            }
            catch (Exception ex)
            {
                log($"❌ Failed to load {what}: {ex.Message}");
                return null;
            }
        }

        private static async Task<string?> GetPublicIpAsync()
        {
            foreach (string service in IpServices)
            {
                try
                {
                    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    string ip = (await Downloader.Http.GetStringAsync(service, timeout.Token)).Trim();
                    if (IPAddress.TryParse(ip, out _))
                        return ip;
                }
                catch
                {
                }
            }
            return null;
        }
    }
}
