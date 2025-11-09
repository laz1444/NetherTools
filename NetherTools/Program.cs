using System.Diagnostics;
using System.Text;
using NetherTools.GUI;
using NetherTools.Memory;

namespace NetherTools
{
    public class Program
    {
        public static IntPtr hProc;

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

            ScrollText();

            Console.WriteLine("Done");
            MainGUI.Run();
        }

        static void ScrollText()
        {
            string text = "NetherToo";
            string sequence = "ls for v1.21.120 NetherToo";
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
