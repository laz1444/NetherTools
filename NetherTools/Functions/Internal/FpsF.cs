using NetherTools.Memory;

namespace NetherTools.Functions.Internal
{
    public class FpsF
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

        private static void Function()
        {
            if (isRunning)
            {
                return;
            }
            isRunning = true;

            while (isRunning)
            {
                byte[] coordinateBytes = MemoryReader.ReadBytes(DynamicMemory.fps, 4);
                Player.FPS = FromBytes.ToFloat(coordinateBytes);
                Thread.Sleep(2000);
            }
        }
    }
}
