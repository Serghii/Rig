
namespace Rig.Telegram.TeleCommand
{
    class StopAllPingTCmd : TCommandBase, ITCommand
    {
        public StopAllPingTCmd(ITCommandService srv) : base(srv){}

        public TCmdType Type => TCmdType.stopAllPing;
        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override string Name => TeleSettings.stopsAllPing;

        public void Execute()
        {
            srv.Ctrl.SetAllPingActivity(false);
        }
    }
}
