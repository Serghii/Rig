namespace Rig.Telegram.TeleCommand
{
    public interface ITCommand
    {
        TCmdType Type { get; }
        bool Is(string command);
        void Execute();
        void Execute(JsonData jd);
        string Name { get; }
    }
}
