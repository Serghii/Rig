using System;

namespace Rig.Telegram.TeleCommand
{
    class PcShutDownTCmd : TCommandBase, ITCommand
    {
        public PcShutDownTCmd(ITCommandService srv) : base(srv)
        {
        }

        public TCmdType Type => TCmdType.pcShutdown;
                
        public void Execute()
        {
            RigEx.WriteLineColors("Command ShutDown".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Ctrl.PCShutdownButton();
        }

        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override string Name => TeleSettings.pcshutdown;
    }
}
