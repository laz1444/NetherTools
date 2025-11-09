using System.Runtime.InteropServices;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

namespace NetherTools.GUI
{
    public class Overlay
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr CreateWindowEx(
            int dwExStyle, string lpClassName, string lpWindowName,
            int dwStyle, int x, int y, int nWidth, int nHeight,
            IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        static extern bool TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [StructLayout(LayoutKind.Sequential)]
        struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public int pt_x;
            public int pt_y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        const int WS_POPUP = unchecked((int)0x80000000);
        const int WS_VISIBLE = 0x10000000;
        const int WS_EX_TOPMOST = 0x00000008;
        const int WS_EX_LAYERED = 0x00080000;
        const int WS_EX_TRANSPARENT = 0x00000020;

        const int SW_SHOW = 5;
        const int WM_DESTROY = 0x0002;
        const int WM_PAINT = 0x000F;

        private static WindowRenderTarget _target;
        private static SolidColorBrush _brush;

        static IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam) //todo not working like supposed to. manual trigger (hack) is at line 135
        {
            switch (msg)
            {
                case WM_PAINT:
                    DrawCrosshair();
                    break;
                case WM_DESTROY:
                    PostQuitMessage(0);
                    break;
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public static void CreateOverlay(int x, int y, int width, int height)
        {
            var wc = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProcDelegate)WindowProc),
                hInstance = Marshal.GetHINSTANCE(typeof(Overlay).Module),
                lpszClassName = "OverlayClass"
            };
            RegisterClassEx(ref wc);

            IntPtr hWnd = CreateWindowEx(
                WS_EX_TOPMOST | WS_EX_LAYERED | WS_EX_TRANSPARENT,
                wc.lpszClassName, "NetherTools",
                WS_POPUP | WS_VISIBLE,
                x, y, width, height,
                IntPtr.Zero, IntPtr.Zero, wc.hInstance, IntPtr.Zero);

            ShowWindow(hWnd, SW_SHOW);

            var factory = new SharpDX.Direct2D1.Factory();
            var renderProps = new HwndRenderTargetProperties
            {
                Hwnd = hWnd,
                PixelSize = new SharpDX.Size2(width, height),
                PresentOptions = PresentOptions.None
            };

            _target = new WindowRenderTarget(factory,
                new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)),
                renderProps);

            _brush = new SolidColorBrush(_target, new RawColor4(1f, 0f, 0f, 1f));

            MSG msg;
            while (GetMessage(out msg, IntPtr.Zero, 0, 0))
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
                InvalidateRect(hWnd, IntPtr.Zero, false);
                Thread.Sleep(16); //around 60fps...
            }

            _brush.Dispose();
            _target.Dispose();
            factory.Dispose();
        }

        static void DrawCrosshair()
        {
            _target.BeginDraw();
            _target.Clear(new RawColor4(0, 0, 0, 1));

            int cx = (int)_target.Size.Width / 2;
            int cy = (int)_target.Size.Height / 2;
            int size = 20;

            _target.DrawLine(new RawVector2(cx - size, cy), new RawVector2(cx + size, cy), _brush, 2f);
            _target.DrawLine(new RawVector2(cx, cy - size), new RawVector2(cx, cy + size), _brush, 2f);

            _target.EndDraw();
        }

        delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}
