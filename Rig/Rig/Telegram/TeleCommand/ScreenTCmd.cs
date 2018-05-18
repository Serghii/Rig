using System;
using Rig.Telegram.Model;

namespace Rig.Telegram.TeleCommand
{
    class ScreenTCmd : TCommandBase, ITCommand
    {
        public ScreenTCmd(ITCommandService srv) : base(srv)
        {
        }

        public TCmdType Type => TCmdType.screen;
        public void Execute()
        {
            RigEx.WriteLineColors("Command screen no parameters ".AddTimeStamp(), ConsoleColor.DarkRed);
        }

        public async void Execute(JsonData jd)
        {
            RigEx.WriteLineColors("Command send screen".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Ctrl.CreateScreenShot();
            foreach (var user in srv.Ctrl.TelegramUser)
            {
                await Bot.Client?.SendPhotoAsync(user.Id, System.IO.File.OpenRead(MainClass.ScreenPath));
            }
        }

        public override string Name => TeleSettings.screen;
    }
}
