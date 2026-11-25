using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public static class SecretHasher
    {
        private const string Scheme = "pbkdf2-sha512";
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int MinIterations = 600_000;
        private const int MaxIterations = 5_000_000;

        public static bool Verify(string secret, string? encoded)
        {
            string[] parts = (encoded ?? "").Split('$');
            if (parts.Length != 4 || parts[0] != Scheme)
                return false;
            if (!int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int iterations)
                || iterations < MinIterations || iterations > MaxIterations)
                return false;

            byte[] salt, expected;
            try
            {
                salt = Convert.FromHexString(parts[2]);
                expected = Convert.FromHexString(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }
            if (salt.Length != SaltSize || expected.Length != KeySize)
                return false;

            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.ASCII.GetBytes(Sha256(secret)), salt, iterations, HashAlgorithmName.SHA512, KeySize);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        private static string Sha256(string text) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();
    }
}
