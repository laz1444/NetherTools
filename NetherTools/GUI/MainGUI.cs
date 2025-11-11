using System.Runtime.InteropServices;
using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace NetherTools.GUI
{
    public class MainGUI
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        public struct RECT { public int Left, Top, Right, Bottom; }

        delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public static GraphicsWindow window = null;

        public static SolidBrush Orange = null;
        public static SolidBrush Red = null;
        public static SolidBrush Green = null;
        public static SolidBrush guiBG = null;
        public static Font font = null;

        public static void Run()
        {
            IntPtr mcHandle = FindWindow(null, "Minecraft");
            if (mcHandle == IntPtr.Zero)
            {
                Log.error("Minecraft window not found!");
                return;
            }

            GetWindowRect(mcHandle, out RECT gameRect);
            int width = gameRect.Right - gameRect.Left;
            int height = gameRect.Bottom - gameRect.Top;
            Log.debug($"Found Minecraft at ({gameRect.Left},{gameRect.Top}) size {width}x{height}");
            Notifications.Send("NetherTools Loaded!");
            var graphics = new Graphics{};

            window = new GraphicsWindow(gameRect.Left, gameRect.Top, width, height, graphics)
            {
                FPS = 60,
                IsTopmost = true,
                IsVisible = true
            };

            window.SetupGraphics += (s, e) =>
            {
                var gfx = e.Graphics;

                if (e.RecreateResources)
                {
                    return;
                }

                Orange = gfx.CreateSolidBrush(255, 132, 0);
                Red = gfx.CreateSolidBrush(255, 0, 0);
                Green = gfx.CreateSolidBrush(0, 255, 0);
                guiBG = gfx.CreateSolidBrush(0, 0, 0, 150);
                font = gfx.CreateFont("Consolas", 15);
            };

            window.DrawGraphics += (s, e) =>
            {
                var gfx = e.Graphics;

                gfx.ClearScene();

                Notifications.Draw(gfx);
                HotKeyWindow.Draw(gfx);
                PlayerCoordinate.Draw(gfx);
            };

            new Thread(() =>
            {
                while (IsWindow(mcHandle))
                {
                    GetWindowRect(mcHandle, out RECT r);

                    window.X = r.Left;
                    window.Y = r.Top;
                    window.Width = r.Right - r.Left;
                    window.Height = r.Bottom - r.Top;

                    window.IsVisible = GetForegroundWindow() == mcHandle;

                    Thread.Sleep(100);
                }
                window.Dispose();
            })
            { IsBackground = true }.Start();

            window.Create();
            window.Join();
        }
    }
}
