using System;

namespace Rig.Telegram.TeleCommand
{
    class PcRestartTCmd : TCommandBase, ITCommand
    {
        public PcRestartTCmd(ITCommandService srv) : base(srv)
        {
        }

        public TCmdType Type => TCmdType.pcRestart;

        public void Execute()
        {
            RigEx.WriteLineColors("Command Restart PC".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Ctrl.PCRestartButton();
        }

        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override string Name => TeleSettings.restartMiner;
    }
}
