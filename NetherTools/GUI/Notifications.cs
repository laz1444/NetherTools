using GameOverlay.Drawing;

namespace NetherTools.GUI
{
    public static class Notifications
    {
        private static List<NotificationMessage> messages = new List<NotificationMessage>();

        public static void Draw(Graphics gfx)
        {
            messages.RemoveAll(m => (DateTime.Now - m.Created).TotalSeconds > m.TimeSec);

            float y = 40;
            foreach (var msg in messages)
            {
                var size = gfx.MeasureString(MainGUI.font, msg.Text);

                gfx.FillRectangle(MainGUI.guiBG, 18, y, size.X + 20, size.Y + y);
                gfx.DrawText(MainGUI.font, MainGUI.Orange, 20, y, msg.Text);

                y += 20;
            }
        }

        public static void Send(string message, int timeInSecounds = 5)
        {
            messages.Add(new NotificationMessage(message, timeInSecounds));
        }
    }

    public class NotificationMessage
    {
        public string Text { get; set; }
        public int TimeSec { get; set; }
        public DateTime Created { get; set; }
        public NotificationMessage(string text, int timeSec)
        {
            Text = text;
            Created = DateTime.Now;
            TimeSec = timeSec;
        }
    }
}
