using System.Drawing;
using GameOverlay.Drawing;

namespace NetherTools.GUI
{
    public class HotKeyWindow
    {
        public static bool enabled = false;

        private static string title = "---------------------------- Available Hot Keys ----------------------------";
        private static string line1 = " TAB + C  Player position                 TAB + F  FPS";
        private static string line2 = " Insert   This GUI";

        public static void Draw(Graphics gfx)
        {
            if (!enabled)
            {
                return;
            }

            var size = gfx.MeasureString(MainGUI.font, title);

            float paddingX = 10f;
            float paddingY = 6f;

            float centerX = MainGUI.window.Width / 2f;
            float centerY = (MainGUI.window.Height / 2f) - 100;

            float rectWidth = size.X + 2 * paddingX;
            float rectHeight = size.Y + 2 * paddingY;

            float rectLeft = centerX - rectWidth / 2f;
            float rectTop = centerY - rectHeight / 2f;

            gfx.FillRectangle(MainGUI.guiBG, rectLeft, rectTop, rectLeft + rectWidth, rectTop + rectHeight + 60);

            gfx.DrawText(MainGUI.font, MainGUI.Orange, centerX - size.X / 2f, centerY - size.Y / 2f, title);
            gfx.DrawText(MainGUI.font, MainGUI.Orange, centerX - size.X / 2f, (centerY - size.Y / 2f) + 40, line1);
            gfx.DrawText(MainGUI.font, MainGUI.Orange, centerX - size.X / 2f, (centerY - size.Y / 2f) + 60, line2);

            gfx.DrawText(MainGUI.font, PlayerCoordinate.enabled ? MainGUI.Green : MainGUI.Red, (centerX - size.X / 2f) + 250, (centerY - size.Y / 2f) + 40, PlayerCoordinate.enabled ? "[ON]" : "[OFF]");
            gfx.DrawText(MainGUI.font, MainGUI.Green, (centerX - size.X / 2f) + 250, (centerY - size.Y / 2f) + 60, "[ON]");
        }
    }
}
