using System.Runtime.InteropServices;
using WindowsInput;

namespace NetherTools.GUI
{
    public static class KeyboardInput
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        private const int INSERT = 0x2D;
        private const int ALT = 0x12;
        private const int C = 0x43;

        public static void Run()
        {
            var inputSimulator = new InputSimulator();

            while (true)
            {
                if (IsKeyPressed(INSERT))
                {
                    HotKeyWindow.enabled = !HotKeyWindow.enabled;
                    Thread.Sleep(500);
                }

                if (IsKeyPressed(ALT) && IsKeyPressed(C))
                {
                    PlayerCoordinate.enabled = !PlayerCoordinate.enabled;
                    Thread.Sleep(500);
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
