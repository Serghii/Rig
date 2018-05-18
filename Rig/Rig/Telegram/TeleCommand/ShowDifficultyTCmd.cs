using System;
using System.Linq;

namespace Rig.Telegram.TeleCommand
{
    class ShowDifficultyTCmd : TCommandBase, ITCommand
    {
        public ShowDifficultyTCmd(ITCommandService srv) : base(srv)
        {
        }
        
        public TCmdType Type => TCmdType.showDifficulty;
        public override string Name => TeleSettings.difficulty;

        public void Execute()
        {
            RigEx.WriteLineColors($"Command  {Name}", ConsoleColor.Yellow);
            var d = srv.Ctrl.GetDiffucalty();
            string diff = d.Aggregate(String.Empty, (current, d1) => current + $"\n{d1.Key}:\t{d1.Value}");
            srv.SendMsg(diff);
        }

        public void Execute(JsonData jd)
        {
            Execute();
        }
    }
}
