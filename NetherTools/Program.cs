using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
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
            string procName = "Minecraft.Windows";
            Process[] procs = Process.GetProcessesByName(procName);
            if (procs.Length == 0)
            {
                Console.WriteLine($"Process {procName} not found");
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
            Console.WriteLine($"Injecting...");

            DynamicMemory.version = MemoryReader.ScanMemory(hProc, Hooks.version);

            if (DynamicMemory.version == IntPtr.Zero)
            {
                Console.WriteLine("error versionAddress");
                return;
            }

            Thread titleMonitor = new Thread(ScrollText);
            titleMonitor.Start();
            Console.WriteLine("Done");
            IntPtr console = GetConsoleWindow();
            SetWindowPos(console, HWND_BOTTOM, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010 | 0x0040);
            MainGUI.Run();
        }

        static void ScrollText()
        {
            string text = "NetherToo";
            string sequence = "ls for v1.21.121 NetherToo";
            int seqIndex = 0;

            while (true)
            {
                MemoryWriter.Write(DynamicMemory.version, Encoding.ASCII.GetBytes(text));
                text = text.Substring(1);
                text += sequence[seqIndex];
                seqIndex = (seqIndex + 1) % sequence.Length;

                Thread.Sleep(1000);
            }
        }
    }
}
