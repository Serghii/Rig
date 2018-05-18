using System;

namespace Rig.Telegram.TeleCommand
{
    class StopMinerTCmd : TCommandBase, ITCommand
    {
        public StopMinerTCmd(ITCommandService srv) : base(srv){}
        public TCmdType Type => TCmdType.stopMining;
        public override string Name => TeleSettings.startMining;

        public void Execute()
        {
            RigEx.WriteLineColors($"Command Stop miner".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Ctrl.MinerCommand(CurrentMinerCommand.stopMiner);
        }

        public void Execute(JsonData jd)
        {
           Execute();
        }
    }
}
