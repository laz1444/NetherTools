using System.Text;
using NetherTools.Memory;

namespace NetherTools.Functions.Internal
{
    public class VersionF
    {
        private static Timer timer;
        public static bool isRunning => timer != null;

        public static void Run()
        {
            string text = "NetherToo";
            string sequence = "ls for v1.21.124 NetherToo";
            int seqIndex = 0;

            timer = new Timer(_ =>
            {
                MemoryWriter.Write(DynamicMemory.version, Encoding.ASCII.GetBytes(text));
                text = text.Substring(1);
                text += sequence[seqIndex];
                seqIndex = (seqIndex + 1) % sequence.Length;
            }, null, 0, 1000);
        }

        public static void Stop()
        {
            timer?.Dispose();
            timer = null;
        }
    }
}
