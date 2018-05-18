using System;

namespace Rig.Telegram.TeleCommand
{
    class UpdateTCmd : TCommandBase, ITCommand
    {
        public TCmdType Type => TCmdType.UpdateGSheet;

        public UpdateTCmd(ITCommandService srv) : base(srv){}
        public void Execute()
        {
            RigEx.WriteLineColors("Command Update Data".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Ctrl.UpdateData();
        }

        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override string Name => TeleSettings.update;
    }
}
