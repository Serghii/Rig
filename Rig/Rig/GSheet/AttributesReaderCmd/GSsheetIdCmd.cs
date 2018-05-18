using System;
using System.Collections.Generic;
using System.Linq;
using Rig;

class GSsheetIdCmd : AttributesReaderBaseCmd
{
    public GSsheetIdCmd(IGSheet gSheet) : base(gSheet) { }

    public override string GetAttributesType => GShSettings.sheetId;
    public override void Execute(IList<object> line)
    {
        if (line == null || line.Count < 2)
        {
            RigEx.WriteLineColors("CoinName parametr error ".AddTimeStamp(), ConsoleColor.DarkRed);
            return;
        }

        int id;
        if (!int.TryParse(line[2].ToString(), out id))
        {
            Console.WriteLine($"line {line[0]} senver name {line[1]} do not have id as int {line[2]} ");
        }

        string serverName = line[1].ToString().ToLower();
        var newSheed = new ServerInfo(id, serverName, true);

         if (serverName.Contains(GShSettings.PCname))
        {
            gSheet.MyPage.MyServerSheetId = newSheed;
        }
        else if(!gSheet.Data.ServersSheetId.Any(o => o.Name.Contains(serverName)))
        {
            gSheet.Data.ServersSheetId.Add(newSheed);
        }
    }
}