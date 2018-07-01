using System;
using Rig.Telegram.Model;
using Rig.Telegram.TeleCommand;

namespace Rig.Telegram
{
    class MinerTCmd : TCommandBase, ITCommand
    {
        public MinerTCmd(ITCommandService srv) : base(srv){}
        public override string Name => TeleSettings.miner;
        public TCmdType Type => TCmdType.miner;
        public void Execute(JsonData jd)
        {
            Execute();
        }

        public void Execute()
        {
            RigEx.WriteLineColors($"Command {Name}".AddTimeStamp(), ConsoleColor.Cyan);
            try
            {
                foreach (IToken token in srv.Ctrl.TelegramUser)
                  Bot.Client?.SendTextMessageAsync(token.Id, srv?.CurMiner?.Name, replyMarkup: srv.GetMinersInlinekeyBoard());
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Telegram Send message Error: {e.Message}".AddTimeStamp(),ConsoleColor.DarkRed);   
                throw;
            }
        }
    }
}
