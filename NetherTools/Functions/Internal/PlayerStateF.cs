using System.Text;
using NetherTools.GUI;
using NetherTools.Memory;

namespace NetherTools.Functions.Internal
{
    public class PlayerStateF
    {
        public static bool isRunning { get; protected set; } = false;
        private static bool wasInMenu = false;
        private static bool didMenuScan = false;
        private static bool inputDetectRun = false;

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
                byte[] stateBytes = MemoryReader.ReadBytes(DynamicMemory.playerState, 4);
                Player.PlayerState = Encoding.UTF8.GetString(stateBytes);

                if (Player.PlayerState == "Menu")
                {
                    wasInMenu = true;
                    didMenuScan = false;
                    VersionF.Run();
                    PositionF.Stop();
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
                        Thread.Sleep(6000);
                        didMenuScan = true;
                        MemoryScanner.Scan(MemoryScanner.ScanType.Game);
                        Notifications.Send("NetherTools: Click Insert to see available options", 10);
                        if (!inputDetectRun)
                        {
                            inputDetectRun = true;
                            Thread inputDetect = new Thread(KeyboardInput.Run);
                            inputDetect.Start();
                        }
                    }
                    VersionF.Stop();
                }

                Thread.Sleep(6000);
            }
        }
    }
}
