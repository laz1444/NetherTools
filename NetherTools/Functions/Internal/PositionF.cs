
using NetherTools.Memory;

namespace NetherTools.Functions.Internal
{
    public class PositionF
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
                byte[] coordinateBytes = MemoryReader.ReadBytes(DynamicMemory.movement, 4 * 3, 2 + (16 * 9));
                Log.debug($"pos: {FromBytes.ToFloat(coordinateBytes)}, {FromBytes.ToFloat(coordinateBytes, 4)}, {FromBytes.ToFloat(coordinateBytes, 8)}");
                Thread.Sleep(2000);
            }
        }
    }
}
