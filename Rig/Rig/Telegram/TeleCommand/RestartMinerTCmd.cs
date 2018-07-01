using System;
using System.Runtime.InteropServices;

namespace Rig.Telegram.TeleCommand
{
    class RestartMinerTCmd : TCommandBase, ITCommand
    {
        public RestartMinerTCmd(ITCommandService srv) : base(srv)
        {
        }

        public TCmdType Type => TCmdType.restartMiner;

        public void Execute(JsonData jd)
        {
            Execute();
        }

        public void Execute()
        {
            RigEx.WriteLineColors("Command restartMiner".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Ctrl.RestartMinerButton();
        }

        public override string Name => TeleSettings.restartMiner;
    }
}
