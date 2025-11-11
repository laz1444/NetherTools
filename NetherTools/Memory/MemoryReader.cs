using System.Runtime.InteropServices;

namespace NetherTools.Memory
{
    public static class MemoryReader
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, IntPtr dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        public static byte[] ReadBytes(IntPtr address, int size, int offset = 0)
        {
            byte[] buffer = new byte[size];
            IntPtr targetAddress = (IntPtr)(address.ToInt64() + offset);

            if (ReadProcessMemory(Program.hProc, targetAddress, buffer, (IntPtr)size, out IntPtr bytesRead))
            {
                return buffer;
            }

            throw new Exception($"Failed to read memory at 0x{targetAddress.ToInt64():X}");
        }

        private static IntPtr PatternScan(byte[] buffer, IntPtr baseAddress, int bufferSize, string pattern)
        {
            string[] patternBytes = pattern.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i <= bufferSize - patternBytes.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < patternBytes.Length; j++)
                {
                    if (patternBytes[j] == "?" || patternBytes[j] == "??")
                        continue;

                    byte patternByte = Convert.ToByte(patternBytes[j], 16);
                    if (buffer[i + j] != patternByte)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return (IntPtr)(baseAddress.ToInt64() + i);
            }

            return IntPtr.Zero;
        }

        public static Dictionary<string, IntPtr> ScanMemory(IntPtr hProc, Dictionary<string, string> patterns)
        {
            Log.info("Scan in progress, expected performace isues until done");

            var results = new Dictionary<string, IntPtr>();

            long minAddress = 0x10000000000;
            long maxAddress = 0x4FFFFFFFFFF;
            MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();
            long address = minAddress;

            while (address < maxAddress && VirtualQueryEx(hProc, (IntPtr)address, out memInfo, (uint)Marshal.SizeOf(memInfo)) != 0)
            {
                if ((memInfo.State & 0x1000) != 0 && (memInfo.Protect & 0x100) == 0)
                {
                    IntPtr regionBase = memInfo.BaseAddress;
                    int regionSize = (int)memInfo.RegionSize;

                    if (regionSize > 0 && regionSize < 100 * 1024 * 1024)
                    {
                        byte[] regionData = new byte[regionSize];
                        if (ReadProcessMemory(hProc, regionBase, regionData, (IntPtr)regionSize, out IntPtr bytesRead))
                        {
                            foreach (var pattern in patterns)
                            {
                                if (!results.ContainsKey(pattern.Key))
                                {
                                    IntPtr foundAddr = PatternScan(regionData, regionBase, regionSize, pattern.Value);
                                    if (foundAddr != IntPtr.Zero)
                                    {
                                        results[pattern.Key] = foundAddr;
                                        Log.debug($"Found '{pattern.Key}' at: 0x{foundAddr.ToInt64():X}");

                                        if (results.Count == patterns.Count)
                                            return results;
                                    }
                                }
                            }
                        }
                    }
                }
                address = (long)memInfo.BaseAddress + (long)memInfo.RegionSize;
            }

            return results;
        }
    }
}
