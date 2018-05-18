namespace Rig.Telegram.TeleCommand
{
    public abstract class TCommandBase
    {
        protected ITCommandService srv;

        protected TCommandBase (ITCommandService srv)
        {
            this.srv = srv;
        }
        public abstract string Name { get; }
        public virtual bool Is(string command)
        {
            return Name == command;
        }
    }
}
