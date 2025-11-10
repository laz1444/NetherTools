using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NetherTools.Functions;
using NetherTools.Functions.Internal;
using NetherTools.GUI;
using NetherTools.Memory;

namespace NetherTools
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        public static IntPtr hProc;
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        static void Main(string[] args)
        {
            Log.debugMode = true;
            string procName = "Minecraft.Windows";
            Process[] procs = Process.GetProcessesByName(procName);
            if (procs.Length == 0)
            {
                Log.error($"Process {procName} not found");
                return;
            }

            Process proc = procs[0];

            if (!Process2.GetProcess(proc.Id))
            {
                return;
            }

            ProcessModule mainMod = proc.MainModule;
            IntPtr moduleBase = mainMod.BaseAddress;
            int moduleSize = mainMod.ModuleMemorySize;
            Log.info($"Injecting... Please stay in Main menu until done");

            MemoryScanner.Scan(MemoryScanner.ScanType.Menu);

            IntPtr console = GetConsoleWindow();
            SetWindowPos(console, HWND_BOTTOM, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010 | 0x0040);
            MainGUI.Run();
        }
    }
}
