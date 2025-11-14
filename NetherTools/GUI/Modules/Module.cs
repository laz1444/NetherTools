namespace NetherTools.GUI.Modules
{
    public abstract class Module
    {
        public string Value { get; protected set; } = "";
        public string Name { get; protected set; } = "";
        public virtual bool Enabled { get; protected set; } = false;

        protected Module(string name) => Name = name;

        public abstract void Toggle();
    }
}
