using NetherTools.Memory;

namespace NetherTools.Functions.Internal
{
    public class PositionF
    {
        private static Timer timer;
        public static bool isRunning => timer != null;

        public static void Run()
        {
            timer = new Timer(_ =>
            {
                byte[] coordinateBytes = Hooks.GetPlayerCoordinates();
                Player.PlayerPosition.X = FromBytes.ToFloat(coordinateBytes);
                Player.PlayerPosition.Y = FromBytes.ToFloat(coordinateBytes, 4);
                Player.PlayerPosition.Z = FromBytes.ToFloat(coordinateBytes, 8);
            }, null, 0, 2000);
        }

        public static void Stop()
        {
            timer?.Dispose();
            timer = null;
        }
    }
}
