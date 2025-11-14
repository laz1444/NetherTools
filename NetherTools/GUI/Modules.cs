using GameOverlay.Drawing;
using NetherTools.GUI.Modules;

namespace NetherTools.GUI
{
    public static class ModulesProcessor
    {
        private static List<Module> registry = new List<Module>();
        private static List<Module> data = new List<Module>();
        private static readonly object _lock = new object();

        public static void Draw(Graphics gfx)
        {
            float y = 40;
            foreach (var module in Get())
            {
                var size = gfx.MeasureString(MainGUI.font, module.Value);

                float backgroundWidth = size.X + 20;
                float posX = MainGUI.window.Width - backgroundWidth - 20;

                gfx.FillRectangle(MainGUI.guiBG, posX + 15, y, posX + backgroundWidth + 5, y + size.Y);
                gfx.DrawText(MainGUI.font, MainGUI.Orange, posX + 20, y, module.Value);

                y += 20;
            }
        }

        public static void Add(Module module)
        {
            lock (_lock)
            {
                data.Add(module);
            }
        }

        public static void Remove(Module module)
        {
            lock (_lock)
            {
                data.Remove(module);
            }
        }

        public static IEnumerable<Module> Get()
        {
            lock (_lock)
            {
                return data.ToList();
            }
        }

        public static void Register(Module module)
        {
            registry.Add(module);
        }

        public static Module? Get(string name)
        {
            lock (registry)
            {
                return registry.FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
