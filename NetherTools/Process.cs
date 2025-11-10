using System.Runtime.InteropServices;

namespace NetherTools
{
    public static class Process2
    {
        const uint PROCESS_VM_READ = 0x0010;
        const uint PROCESS_VM_WRITE = 0x0020;
        const uint PROCESS_VM_OPERATION = 0x0008;
        const uint PROCESS_QUERY_INFORMATION = 0x0400;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        public static bool GetProcess(int processId)
        {
            IntPtr hProc = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, processId);
            if (hProc == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                Log.error($"Couldn't get process {err}");
                return false;
            }
            Program.hProc = hProc;
            return true;
        }
    }
}
