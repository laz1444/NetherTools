using NetherTools.Memory;

namespace NetherTools.Functions.Internal
{
    public class FpsF
    {
        private static Timer timer;
        public static bool isRunning => timer != null;

        public static void Run()
        {
            timer = new Timer(_ =>
            {
                byte[] fpsBytes = Hooks.GetFPS();
                Player.FPS = FromBytes.ToFloat(fpsBytes);
            }, null, 0, 2000);
        }

        public static void Stop()
        {
            timer?.Dispose();
            timer = null;
        }
    }
}
