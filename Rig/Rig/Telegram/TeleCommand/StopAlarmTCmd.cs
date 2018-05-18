
namespace Rig.Telegram.TeleCommand
{
    class StopAlarmTCmd : TCommandBase, ITCommand
    {
        public StopAlarmTCmd(ITCommandService srv) : base(srv){}

        public TCmdType Type => TCmdType.StopAlarm;
        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override string Name => TeleSettings.stopAlarm;

        public void Execute()
        {
            srv.Ctrl.SetAllAlarmActivity(false);
        }
    }
}
