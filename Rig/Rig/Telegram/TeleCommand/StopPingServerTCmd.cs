using System;

namespace Rig.Telegram.TeleCommand
{
    class StopPingServerTCmd : TCommandBase, ITCommand
    {
        public StopPingServerTCmd(ITCommandService srv) : base(srv)
        {
        }

        public TCmdType Type =>TCmdType.stopPing;
        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void Execute(JsonData jd)
        {
            RigEx.WriteLineColors($"Command StopMiner".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Ctrl.SetPingActivityFor(jd.Value, false);
        }

        public override string Name => TeleSettings.stopPing;
    }
}
