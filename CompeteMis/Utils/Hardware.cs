using Compete.Mis;
using System;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Compete.Utils
{
    internal static class Hardware
    {
        public static string? GetProcessorId()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            var collection = searcher.Get();
            foreach (var obj in collection)
                if (obj["ProcessorId"] is not null)
                    return obj["ProcessorId"].ToString();
            return null;
        }

        public static string? GetMainboardId()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            var collection = searcher.Get();
            foreach (var obj in collection)
                if (obj["SerialNumber"] is not null)
                    return obj["SerialNumber"].ToString();
            return null;
        }

        public static string? GetDiskId()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            var collection = searcher.Get();
            foreach (var obj in collection)
                if (obj["SerialNumber"] is not null)
                    return obj["SerialNumber"].ToString();
            return null;
        }

        public static string GetPhysicalAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                if (nic.OperationalStatus == OperationalStatus.Up)
                    return nic.GetPhysicalAddress().ToString();

            return string.Empty;
        }

        [DllImport("CompeteLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getCpuId();

        public static string GetCpuId() => GlobalCommon.ConvertString(getCpuId());

        [DllImport("CompeteLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getMotherboardId();

        public static string GetMotherboardId() => GlobalCommon.ConvertString(getMotherboardId());

        [DllImport("CompeteLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getDiskId();

        public static string GetHardDiskId() => GlobalCommon.ConvertString(getDiskId());

        [DllImport("CompeteLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr getMacAddress();

        public static string GetMacAddress() => GlobalCommon.ConvertString(getMacAddress());
    }
}
