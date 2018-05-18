using System;
using Rig.Telegram.Model;
using Telegram.Bot.Types.Enums;

namespace Rig.Telegram.TeleCommand
{
    class MainKeyboardKTCmd : TCommandBase, ITCommand
    {
        public MainKeyboardKTCmd(ITCommandService srv) : base(srv){}
        public TCmdType Type => TCmdType.mainKeybord;
        public void Execute(JsonData jd)
        {
            Execute();
        }

        public override bool Is(string command)
        {
            return command == Name || command.ToLower() == Name;
        }

        public override string Name => TeleSettings.k;
        public void Execute()
        {
            RigEx.WriteLineColors($"Command show keyboard".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Markup.ResizeKeyboard = true;
            srv.InitHomeKeybord();
            srv.Markup.Keyboard = srv.HomeKeyButton;
            try
            {
                foreach (IToken token in srv.Ctrl.TelegramUser)
                {
                    Bot.Client?.SendTextMessageAsync(token.Id, "🎹", ParseMode.Default, false, false, 0, srv.Markup);
                }

            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Telegram: Send keybord  Error: {e.Message}".AddTimeStamp(), ConsoleColor.DarkRed);
            }

        }
    }
}
