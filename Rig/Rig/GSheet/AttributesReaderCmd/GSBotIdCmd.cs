using System;
using System.Collections.Generic;
using Rig;
using Rig.Telegram;


class GSBotIdCmd : AttributesReaderBaseCmd
{
    public GSBotIdCmd(IGSheet gSheet) : base(gSheet) { }

    public override string GetAttributesType => GShSettings.botId;

    public override void Execute(IList<object> line)
    {

        if (line == null || line.Count < 3)
        {
            RigEx.WriteLineColors("Bot Id parametr error ".AddTimeStamp(), ConsoleColor.DarkRed);
            return;
        }
        if (line[1].ToString().ToLower() != TeleSettings.MyName)
            return;
        gSheet.Data.BotId = line[2].ToString();
    }
}