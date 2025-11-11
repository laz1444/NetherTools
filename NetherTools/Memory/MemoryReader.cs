using System.Numerics;
using System.Runtime.InteropServices;

namespace NetherTools.Memory
{
    public static class MemoryReader
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesRead);

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

        public static long workingOffset = 0x10000000000;

        public static byte[] ReadBytes(IntPtr address, int size, int offset = 0)
        {
            if (size <= 0) return Array.Empty<byte>();

            IntPtr targetAddress = (IntPtr)(address.ToInt64() + offset);
            IntPtr unmanaged = IntPtr.Zero;
            try
            {
                unmanaged = Marshal.AllocHGlobal(size);
                if (!ReadProcessMemory(Program.hProc, targetAddress, unmanaged, size, out IntPtr bytesRead))
                {
                    int err = Marshal.GetLastWin32Error();
                    throw new InvalidOperationException($"Failed to read memory at 0x{targetAddress.ToInt64():X}. Win32 error: {err}");
                }

                int read = bytesRead.ToInt32();
                var managed = new byte[read];
                Marshal.Copy(unmanaged, managed, 0, read);
                return managed;
            }
            finally
            {
                if (unmanaged != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(unmanaged);
                }
            }
        }

        private static unsafe IntPtr PatternScan(byte* bufferPtr, int bufferLen, long baseAddress, ParsedPattern pat)
        {
            int patLen = pat.Length;
            if (patLen == 0 || bufferLen < patLen)
            {
                return IntPtr.Zero;
            }

            if (pat.FirstNonWildcardIndex == -1)
            {
                return (IntPtr)baseAddress;
            }

            int idxByte = pat.FirstNonWildcardIndex;
            byte target = pat.FirstNonWildcardByte;

            int vectorSize = Vector<byte>.Count;
            int i = 0;

            for (; i <= bufferLen - vectorSize; i += vectorSize)
            {
                var v = new Vector<byte>(new ReadOnlySpan<byte>(bufferPtr + i, vectorSize));
                var vt = new Vector<byte>(target);
                var cmp = Vector.Equals(v, vt);

                if (!Vector.EqualsAll(cmp, Vector<byte>.Zero))
                {
                    for (int lane = 0; lane < vectorSize; lane++)
                    {
                        if (v[lane] == target)
                        {
                            int pos = i + lane - idxByte;
                            if (pos < 0 || pos + patLen > bufferLen)
                            {
                                continue;
                            }

                            byte* checkPtr = bufferPtr + pos;
                            bool ok = true;
                            for (int j = 0; j < patLen; j++)
                            {
                                if (pat.IsWildcard[j])
                                {
                                    continue;
                                }
                                if (checkPtr[j] != pat.Bytes[j])
                                {
                                    ok = false; break;
                                }
                            }
                            if (ok)
                            {
                                return (IntPtr)(baseAddress + pos);
                            }
                        }
                    }
                }
            }

            int remStart = Math.Max(0, bufferLen - vectorSize);
            for (int r = remStart; r <= bufferLen - 1; r++)
            {
                int pos = r - idxByte;
                if (pos < 0 || pos + patLen > bufferLen)
                {
                    continue;
                }
                if (bufferPtr[r] != target)
                {
                    continue;
                }
                byte* checkPtr = bufferPtr + pos;
                bool ok = true;
                for (int j = 0; j < patLen; j++)
                {
                    if (pat.IsWildcard[j])
                    {
                        continue;
                    }
                    if (checkPtr[j] != pat.Bytes[j]) { ok = false; break; }
                }
                if (ok)
                {
                    return (IntPtr)(baseAddress + pos);
                }
            }

            return IntPtr.Zero;
        }

        public static unsafe Dictionary<string, IntPtr> ScanMemory(IntPtr hProc, Dictionary<string, string> patterns)
        {
            var results = new Dictionary<string, IntPtr>();
            var parsed = patterns.ToDictionary(kv => kv.Key, kv => ParsedPattern.Parse(kv.Value));

            const int chunkSize = 4 * 1024 * 1024;
            int maxPatternLen = parsed.Values.Max(p => p.Length);
            int overlap = Math.Max(0, maxPatternLen - 1);
            int bufferSize = chunkSize + overlap;

            IntPtr unmanagedBuffer = Marshal.AllocHGlobal(bufferSize);
            try
            {
                long maxAddress = 0x4FFFFFFFFFF;
                MEMORY_BASIC_INFORMATION memInfo = new MEMORY_BASIC_INFORMATION();
                long address = workingOffset;

                while (address < maxAddress && VirtualQueryEx(hProc, (IntPtr)address, out memInfo, (uint)Marshal.SizeOf(memInfo)) != IntPtr.Zero)
                {
                    if ((memInfo.State & 0x1000) != 0 && (memInfo.Protect & 0x100) == 0)
                    {
                        long regionBase = memInfo.BaseAddress.ToInt64();
                        long regionSize = (long)memInfo.RegionSize;

                        if (regionSize > 0 && regionSize < 100 * 1024 * 1024)
                        {
                            long offsetInRegion = 0;
                            int prevTail = 0;

                            while (offsetInRegion < regionSize)
                            {
                                int toRead = (int)Math.Min(chunkSize, regionSize - offsetInRegion);
                                long readAddress = regionBase + offsetInRegion - prevTail;
                                if (readAddress < regionBase)
                                {
                                    readAddress = regionBase;
                                }

                                int targetReadSize = toRead + prevTail;
                                if (targetReadSize > bufferSize)
                                {
                                    targetReadSize = bufferSize;
                                }

                                if (ReadProcessMemory(hProc, (IntPtr)readAddress, unmanagedBuffer, (IntPtr)targetReadSize, out IntPtr bytesRead))
                                {
                                    int bytesInt = bytesRead.ToInt32();
                                    byte* bufPtr = (byte*)unmanagedBuffer.ToPointer();

                                    foreach (var kv in parsed)
                                    {
                                        if (results.ContainsKey(kv.Key))
                                        {
                                            continue;
                                        }

                                        IntPtr found = PatternScan(bufPtr, bytesInt, (long)readAddress, kv.Value);
                                        if (found != IntPtr.Zero)
                                        {
                                            results[kv.Key] = found;
                                            Console.WriteLine($"Found {kv.Key} at 0x{found.ToInt64():X}");
                                            if (results.Count == patterns.Count)
                                            {
                                                return results;
                                            }
                                        }
                                    }

                                    if (overlap > 0)
                                    {
                                        int tailStart = Math.Max(0, bytesInt - overlap);
                                        byte* dest = bufPtr + chunkSize;
                                        byte* src = bufPtr + tailStart;
                                        int copyLen = Math.Min(overlap, bytesInt - tailStart);
                                        Buffer.MemoryCopy(src, dest, overlap, copyLen);
                                        prevTail = copyLen;
                                    }
                                    else prevTail = 0;
                                }
                                else
                                {
                                    prevTail = 0;
                                }

                                offsetInRegion += toRead;
                            }
                        }
                    }

                    address = (long)memInfo.BaseAddress + (long)memInfo.RegionSize;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedBuffer);
            }

            return results;
        }

        public sealed class ParsedPattern
        {
            public readonly byte[] Bytes;
            public readonly bool[] IsWildcard;
            public readonly int Length;
            public readonly int FirstNonWildcardIndex;
            public readonly byte FirstNonWildcardByte;

            public ParsedPattern(byte[] bytes, bool[] isWildcard)
            {
                Bytes = bytes;
                IsWildcard = isWildcard;
                Length = bytes.Length;

                int idx = -1;
                byte b = 0;
                for (int i = 0; i < Length; i++)
                {
                    if (!isWildcard[i]) { idx = i; b = bytes[i]; break; }
                }
                FirstNonWildcardIndex = idx;
                FirstNonWildcardByte = b;
            }

            public static ParsedPattern Parse(string pattern)
            {
                var parts = pattern.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var bytes = new byte[parts.Length];
                var isWildcard = new bool[parts.Length];

                for (int i = 0; i < parts.Length; i++)
                {
                    var p = parts[i];
                    if (p == "?" || p == "??")
                    {
                        isWildcard[i] = true;
                        bytes[i] = 0;
                    }
                    else
                    {
                        isWildcard[i] = false;
                        bytes[i] = Convert.ToByte(p, 16);
                    }
                }
                return new ParsedPattern(bytes, isWildcard);
            }
        }
    }
}
