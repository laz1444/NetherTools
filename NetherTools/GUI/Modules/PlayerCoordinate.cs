namespace NetherTools.GUI.Modules
{
    public class PlayerCoordinate : Module
    {
        private Timer? timer;
        public override bool Enabled => timer != null;
        public PlayerCoordinate() : base("PlayerCoordinate") { }

        public override void Toggle()
        {
            if (timer == null)
            {
                timer = new Timer(_ =>
                {
                    Value = $"Position: {(int)Player.PlayerPosition.X}, {(int)Player.PlayerPosition.Y}, {(int)Player.PlayerPosition.Z}";
                }, null, 0, 1000);
                ModulesProcessor.Add(this);
            }
            else
            {
                timer?.Dispose();
                timer = null;
                ModulesProcessor.Remove(this);
            }
        }
    }
}
