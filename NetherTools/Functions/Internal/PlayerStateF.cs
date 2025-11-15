using System.Text;
using NetherTools.GUI;
using NetherTools.Memory;

namespace NetherTools.Functions.Internal
{
    public class PlayerStateF
    {
        private static Timer timer;
        public static bool isRunning => timer != null;
        private static bool wasInMenu = false;
        private static bool didMenuScan = false;

        public static void Run()
        {
            timer = new Timer(_ =>
            {
                byte[] stateBytes = Hooks.GetPlayerState();
                Player.PlayerState = Encoding.UTF8.GetString(stateBytes);

                if (Player.PlayerState == "Menu")
                {
                    wasInMenu = true;
                    didMenuScan = false;
                    VersionF.Run();
                    ModulesProcessor.Reset();
                    HotKeyWindow.enabled = false;
                }
                else
                {
                    if (!wasInMenu)
                    {
                        Log.error("Wasn't in Menu. Please go to Main Menu in game and restart NetherTools");
                        Stop();
                    }
                    if (!didMenuScan)
                    {
                        didMenuScan = true;
                        MemoryScanner.Scan(MemoryScanner.ScanType.Game);
                        Notifications.Send("NetherTools: Click Insert to see available options", 15);
                    }
                    VersionF.Stop();
                }
            }, null, 0, 6000);
        }

        public static void Stop()
        {
            timer?.Dispose();
            timer = null;
        }
    }
}
