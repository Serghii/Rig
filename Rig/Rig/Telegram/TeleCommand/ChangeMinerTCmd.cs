using System;
using System.Linq;

namespace Rig.Telegram.TeleCommand
{
    class ChangeMinerTCmd : TCommandBase, ITCommand
    {
        public ChangeMinerTCmd(ITCommandService srv) : base(srv){}

        public TCmdType Type => TCmdType.changeMiner;
        public override string Name => TeleSettings.changeMiner;

        
        public void Execute()
        {}

        public void Execute(JsonData jd)
        {

            RigEx.WriteLineColors($"Command change miner {srv.Ctrl.CurMiner} to {jd.Value} ".AddTimeStamp(), ConsoleColor.Yellow);
            var serv = srv.Ctrl.Miners.FirstOrDefault(i => i.Name == jd.Value);
            if (serv != null)
            {
                if (srv.Ctrl.CurMiner.Name != serv.Name)
                    srv.Ctrl.MinerCommand(CurrentMinerCommand.ChangeAndRunMiner, jd);
                else if (!srv.Ctrl.MinerStatus)
                {
                    srv.Ctrl.MinerCommand(CurrentMinerCommand.LounchMiner);
                }
                else
                {
                    RigEx.WriteLineColors($"Warning Change miner - Already Running {serv.Name}".AddTimeStamp(), ConsoleColor.DarkYellow);
                    srv.SendMsg($"{serv.Name} : already running");
                }
            }
            else
                RigEx.WriteLineColors($"Warning Change miner - canot fount miner {jd.Value}".AddTimeStamp(), ConsoleColor.DarkRed);
        }
    }
}
