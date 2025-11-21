using System.Runtime.InteropServices;
using NetherTools.Functions.Internal;
using WindowsInput;

namespace NetherTools.GUI
{
    public static class KeyboardInput
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        private const int INSERT = 0x2D;
        private const int TAB = 0x09;
        private const int B = 0x42;
        private const int C = 0x43;
        private const int F = 0x46;

        public static void Run()
        {
            var inputSimulator = new InputSimulator();

            while (true)
            {
                if (Player.PlayerState == "Menu")
                {
                    Thread.Sleep(5000);
                    continue;
                }

                if (IsKeyPressed(INSERT))
                {
                    HotKeyWindow.enabled = !HotKeyWindow.enabled;
                    Thread.Sleep(500);
                }

                if (IsKeyPressed(TAB))
                {
                    if (IsKeyPressed(B))
                    {
                        FullBrightnessF.Toggle();
                        Thread.Sleep(500);
                    }
                    if (IsKeyPressed(C))
                    {
                        ModulesProcessor.Get("PlayerCoordinate")?.Toggle();
                        Thread.Sleep(500);
                    }
                    if (IsKeyPressed(F))
                    {
                        ModulesProcessor.Get("FPS")?.Toggle();
                        Thread.Sleep(500);
                    }
                }

                Thread.Sleep(10);
            }
        }

        static bool IsKeyPressed(int keyCode)
        {
            return (GetAsyncKeyState(keyCode) & 0x8000) != 0;
        }
    }
}
