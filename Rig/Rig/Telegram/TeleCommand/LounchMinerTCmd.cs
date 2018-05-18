using System;

namespace Rig.Telegram.TeleCommand
{
    class LounchMinerTCmd : TCommandBase, ITCommand
    {
        public LounchMinerTCmd(ITCommandService srv) : base(srv){}

        public TCmdType Type => TCmdType.lounchMiner;
        public override string Name => TeleSettings.startMining;

        public void Execute()
        {
            RigEx.WriteLineColors($"Command {Name}".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Ctrl.MinerCommand(CurrentMinerCommand.LounchMiner);
        }
    

        public void Execute(JsonData jd)
        {
            Execute();
        }
    }
}
