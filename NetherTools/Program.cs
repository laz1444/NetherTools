using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NetherTools.Functions;
using NetherTools.GUI;
using NetherTools.GUI.Modules;

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
            System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName(procName);
            if (procs.Length == 0)
            {
                Log.error($"Process {procName} not found");
                return;
            }

            System.Diagnostics.Process proc = procs[0];

            if (!Process2.GetProcess(proc.Id))
            {
                return;
            }

            ProcessModule mainMod = proc.MainModule;
            IntPtr moduleBase = mainMod.BaseAddress;
            int moduleSize = mainMod.ModuleMemorySize;
            Log.info($"Injecting... Please stay in Main menu until done");

            MemoryScanner.Init(moduleBase);
            MemoryScanner.Scan(MemoryScanner.ScanType.Menu);

            ModulesProcessor.Register(new PlayerCoordinate());
            ModulesProcessor.Register(new FPS());

            Thread inputDetect = new Thread(KeyboardInput.Run);
            inputDetect.Start();

            Log.info($"Done");

            MainGUI.Run();
        }
    }
}
