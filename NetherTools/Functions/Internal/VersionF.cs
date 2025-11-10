using NetherTools.Memory;
using System.Text;

namespace NetherTools.Functions.Internal
{
    public class VersionF
    {
        public static bool isRunning { get; protected set; } = false;

        public static void Run()
        {
            Thread thread = new Thread(Function);
            thread.Start();
        }

        public static void Stop()
        {
            isRunning = false;
        }

        static void Function()
        {
            if (isRunning)
            {
                return;
            }
            isRunning = true;

            string text = "NetherToo";
            string sequence = "ls for v1.21.121 NetherToo";
            int seqIndex = 0;

            while (isRunning)
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
