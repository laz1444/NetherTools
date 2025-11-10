using System.Runtime.InteropServices;

namespace NetherTools.GUI
{
    public class MainGUI
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        public struct RECT { public int Left, Top, Right, Bottom; }

        delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

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
            Overlay.CreateOverlay(gameRect.Left, gameRect.Top, width, height);
            Log.debug($"Minecraft found at ({gameRect.Left},{gameRect.Top}) size {width}x{height}");
        }
    }
}
