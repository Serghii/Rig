using System;

namespace Rig.Telegram.TeleCommand
{
    class RestartTCmd : TCommandBase, ITCommand
    {
        public RestartTCmd(ITCommandService srv) : base(srv)
        {
        }

        public TCmdType Type => TCmdType.restart;
        public void Execute()
        {
            RigEx.WriteLineColors("Command restart".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Ctrl.RestartButton();
        }

        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override string Name => TeleSettings.restart;
    }
}
