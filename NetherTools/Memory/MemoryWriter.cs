using System.Runtime.InteropServices;

namespace NetherTools.Memory
{
    public static class MemoryWriter
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        public static void Write(IntPtr address, byte[] value)
        {
            byte[] writeBuf = new byte[value.Length];
            Array.Copy(value, writeBuf, value.Length);
            for (int i = value.Length; i < writeBuf.Length; i++) writeBuf[i] = 0x00;

            bool wrote = WriteProcessMemory(Program.hProc, address, writeBuf, (IntPtr)writeBuf.Length, out IntPtr bytesWritten);
            if (!wrote)
            {
                Log.error($"WriteProcessMemory failed. Win32 error: {Marshal.GetLastWin32Error()}");
                return;
            }
        }
    }
}
