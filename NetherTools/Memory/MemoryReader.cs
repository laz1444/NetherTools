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

        public static IntPtr PatternScan(IntPtr hProc, IntPtr moduleBase, int moduleSize, string pattern)
        {
            byte[] moduleData = new byte[moduleSize];

            if (!ReadProcessMemory(hProc, moduleBase, moduleData, (IntPtr)moduleSize, out IntPtr bytesRead))
            {
                return IntPtr.Zero;
            }

            string[] patternBytes = pattern.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i <= moduleSize - patternBytes.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < patternBytes.Length; j++)
                {
                    if (patternBytes[j] == "?" || patternBytes[j] == "??")
                        continue;

                    byte patternByte = Convert.ToByte(patternBytes[j], 16);
                    if (moduleData[i + j] != patternByte)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    IntPtr foundAddr = (IntPtr)(moduleBase.ToInt64() + i);

                    return foundAddr;
                }
            }

            return IntPtr.Zero;
        }

        public static IntPtr ScanMemory(nint process, string pattern)
        {
            long minAddress = 0x10000000000;
            long maxAddress = 0x7FFFFFFFFFF;

            MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();
            long address = minAddress;

            while (address < maxAddress && VirtualQueryEx(process, (IntPtr)address, out memInfo, (uint)Marshal.SizeOf(memInfo)) != 0)
            {
                if ((memInfo.State & 0x1000) != 0 && (memInfo.Protect & 0x100) == 0)
                {
                    IntPtr regionBase = memInfo.BaseAddress;
                    int regionSize = (int)memInfo.RegionSize;

                    if (regionSize > 0 && regionSize < 100 * 1024 * 1024)
                    {
                        IntPtr foundAddr = PatternScan(process, regionBase, regionSize, pattern);
                        if (foundAddr != IntPtr.Zero)
                        {
                            Console.WriteLine($"Found in memory region: 0x{regionBase.ToInt64():X}-0x{(regionBase.ToInt64() + regionSize):X}");
                            return foundAddr;
                        }
                    }
                }

                address = (long)memInfo.BaseAddress + (long)memInfo.RegionSize;
            }

            return IntPtr.Zero;
        }
    }
}
