using NetherTools.Memory;

namespace NetherTools.Functions.Internal
{
    public class FullBrightnessF
    {
        public static bool isRunning = false;

        public static void Run()
        {
            isRunning = true;
            MemoryWriter.Write(DynamicMemory.rendering_brightness, BitConverter.GetBytes(10.0f));
        }

        public static void Stop()
        {
            isRunning = false;
            MemoryWriter.Write(DynamicMemory.rendering_brightness, BitConverter.GetBytes(1.0f));
        }

        public static void Toggle()
        {
            if (isRunning)
            {
                Stop();
            }
            else
            {
                Run();
            }
        }
    }
}
