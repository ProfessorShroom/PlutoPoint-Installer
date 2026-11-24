using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public sealed class HardwareInfo
    {
        public static readonly bool IsArm64 = RuntimeInformation.OSArchitecture == Architecture.Arm64;

        public bool HasIntelCpu { get; private init; }
        public bool HasAmdCpu { get; private init; }
        public bool HasIntelGpu { get; private init; }
        public bool HasAmdGpu { get; private init; }
        public bool HasNvidiaGpu { get; private init; }

        public static HardwareInfo Detect()
        {
            HashSet<string> gpus = GetGpuVendorIds();
            string cpu = GetCpuText();
            bool CpuIs(params string[] names) => names.Any(n => cpu.Contains(n, StringComparison.OrdinalIgnoreCase));
            return new HardwareInfo
            {
                HasIntelCpu = CpuIs("Intel"),
                HasAmdCpu = CpuIs("AMD", "Ryzen", "Threadripper"),
                HasIntelGpu = gpus.Contains("8086"),
                HasAmdGpu = gpus.Contains("1002"),
                HasNvidiaGpu = gpus.Contains("10DE")
            };
        }

        private static string GetCpuText()
        {
            string text = "";
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name, Manufacturer FROM Win32_Processor");
                foreach (ManagementObject cpu in searcher.Get())
                    text += $"{cpu["Name"]} {cpu["Manufacturer"]} ";
            }
            catch
            {
            }
            return text;
        }

        private static HashSet<string> GetGpuVendorIds()
        {
            var vendors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT PNPDeviceID, CompatibleID FROM Win32_PnPEntity WHERE PNPDeviceID LIKE 'PCI%'");
                foreach (ManagementObject device in searcher.Get())
                {
                    if (device["CompatibleID"] is not string[] compatibleIds) continue;
                    if (!compatibleIds.Any(id => id != null && id.IndexOf("CC_03", StringComparison.OrdinalIgnoreCase) >= 0)) continue;
                    if (device["PNPDeviceID"] is string pnpId)
                    {
                        Match vendor = Regex.Match(pnpId, @"VEN_([0-9A-F]{4})", RegexOptions.IgnoreCase);
                        if (vendor.Success) vendors.Add(vendor.Groups[1].Value);
                    }
                }
            }
            catch
            {
            }
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Caption, AdapterCompatibility, PNPDeviceID FROM Win32_VideoController");
                foreach (ManagementObject controller in searcher.Get())
                {
                    if (controller["PNPDeviceID"] is string pnpId)
                    {
                        Match vendor = Regex.Match(pnpId, @"VEN_([0-9A-F]{4})", RegexOptions.IgnoreCase);
                        if (vendor.Success) vendors.Add(vendor.Groups[1].Value);
                    }
                    string text = $"{controller["Caption"]} {controller["AdapterCompatibility"]}";
                    if (text.IndexOf("NVIDIA", StringComparison.OrdinalIgnoreCase) >= 0) vendors.Add("10DE");
                    if (text.IndexOf("Intel", StringComparison.OrdinalIgnoreCase) >= 0) vendors.Add("8086");
                    if (text.IndexOf("AMD", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        text.IndexOf("Radeon", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        text.IndexOf("Advanced Micro Devices", StringComparison.OrdinalIgnoreCase) >= 0) vendors.Add("1002");
                }
            }
            catch
            {
            }
            return vendors;
        }
    }
}
