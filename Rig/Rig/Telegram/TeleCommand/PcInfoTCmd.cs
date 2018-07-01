using System;
using Rig.Telegram.Model;

namespace Rig.Telegram.TeleCommand
{
    class PcInfoTCmd : TCommandBase, ITCommand
    {
        public PcInfoTCmd(ITCommandService srv) : base(srv) { }
        public TCmdType Type => TCmdType.pc;
        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override string Name => TeleSettings.infopc;

        public void Execute()
        {
            RigEx.WriteLineColors($"Command show PC".AddTimeStamp(), ConsoleColor.Yellow);
            try
            {
                foreach (IToken token in srv.Ctrl.TelegramUser)
                    Bot.Client?.SendTextMessageAsync(token.Id, "pc",replyMarkup: srv.PcInlinekeyBoard);
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Send message Error: {e.Message}".AddTimeStamp(), ConsoleColor.DarkRed);
            }


        }
    }
}
