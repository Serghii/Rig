namespace Rig.Telegram.TeleCommand
{
    class RunAllTCmd : TCommandBase, ITCommand
    {
        public RunAllTCmd(ITCommandService srv) : base(srv){}
        public TCmdType Type => TCmdType.runAll;
        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override string Name => TeleSettings.runAll;

        public void Execute()
        {
            srv.Ctrl.SetAllPingActivity(true);
            srv.Ctrl.SetAllAlarmActivity(true);
        }
    }
}
