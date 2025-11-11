using GameOverlay.Drawing;

namespace NetherTools.GUI
{
    public static class PlayerCoordinate
    {
        public static bool enabled = false;

        public static void Draw(Graphics gfx)
        {
            if (!enabled)
            {
                return;
            }

            string text = $"Position: {(int)Player.PlayerPosition.X}, {(int)Player.PlayerPosition.Y}, {(int)Player.PlayerPosition.Z}";
            var size = gfx.MeasureString(MainGUI.font, text);

            float backgroundWidth = size.X + 20;
            float posX = MainGUI.window.Width - backgroundWidth - 20;

            gfx.FillRectangle(MainGUI.guiBG, posX + 15, 40, posX + backgroundWidth + 5, 58.5f);
            gfx.DrawText(MainGUI.font, MainGUI.Orange, posX + 20, 40, text);
        }
    }
}
