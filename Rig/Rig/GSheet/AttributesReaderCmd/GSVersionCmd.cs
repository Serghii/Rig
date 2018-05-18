using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rig;
using Rig.Telegram;


class GSVersionCmd : AttributesReaderBaseCmd
{
    public GSVersionCmd(IGSheet gSheet) : base(gSheet) { }

    public override string GetAttributesType => GShSettings.version;

    public override void Execute(IList<object> line)
    {
        if (line == null || line.Count < 3)
        {
            RigEx.WriteLineColors("Bot Id parametr error ".AddTimeStamp(), ConsoleColor.DarkRed);
            return;
        }
        if (line[1].ToString().ToLower() != TeleSettings.MyName)
            return;

        int curVersion;
        if (GetVersionString(line, out curVersion))
        {
            if (!gSheet.Data.VersionIsLiquid(curVersion, gSheet.NewVersion))
            {
                return;
            }
            else
            {
                RigEx.WriteLineColors($"Google Sheet you use last version: {curVersion}", ConsoleColor.Green);
            }

        }
        else
        {
            RigEx.WriteLineColors("Google Sheet cannot read cur version from C", ConsoleColor.Red);
        }
    }

    private bool GetVersionString(IList<object> data, out int curVersion)
    {
        if (data != null  && data.Count >= 3
            && int.TryParse(data[2].ToString(), out curVersion))
        {
            return true;
        }
        else
        {
            curVersion = 0;
            return false;
        }
    }
}