namespace NetherTools.GUI.Modules
{
    public class FPS : Module
    {
        private Timer? timer;
        public override bool Enabled => timer != null;
        public FPS() : base("FPS") { }

        public override void Toggle()
        {
            if (timer == null)
            {
                timer = new Timer(_ =>
                {
                    Value = $"FPS: {Math.Round((decimal)Player.FPS, 2)}";
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
