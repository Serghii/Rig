using System;
using System.Collections.Generic;
using Rig;


class GSUserTokenCmd : AttributesReaderBaseCmd
{
    public GSUserTokenCmd(IGSheet gSheet) : base(gSheet) { }

    public override string GetAttributesType => GShSettings.UserTokenLine;
    public override void Execute(IList<object> line)
    {
        if (line == null || line.Count < 2)
        {
            RigEx.WriteLineColors("CoinName parametr error ".AddTimeStamp(), ConsoleColor.DarkRed);
            return;
        }
        gSheet.Data.UserToKen.Add(GetUserTokenFromLine(line));
    }
    private ServerInfo GetUserTokenFromLine(IList<object> line)
    {
        string name = string.Empty;
        long id;
        bool isActive = true;
        if (line[0] == null)
            Console.WriteLine($"line do not have token name");
        else
            name = line[1].ToString();

        if (!long.TryParse(line[2].ToString(), out id))
        {
            Console.WriteLine($"can not convert to int token line {line[0]}");
            isActive = false;
        }

        return new ServerInfo(id, name, isActive);
    }
}