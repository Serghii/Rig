using System;
using System.Collections.Generic;
using Rig;

class GSCoinCmd : AttributesReaderBaseCmd
{
    public GSCoinCmd(IGSheet gSheet) : base (gSheet) {}

    public override string GetAttributesType => GShSettings.CoinName;
    public override void Execute(IList<object> line)
    {
        if (line == null || line.Count < 2)
        {
            RigEx.WriteLineColors("CoinName parametr error ".AddTimeStamp(),ConsoleColor.DarkRed);
            return;
        }
        int hashRate ;
        int.TryParse(line[2].ToString(), out hashRate);
        hashRate = hashRate == 0 ? 1 : hashRate;
        string coinName = line[1].ToString();
        var coin = new Coin(coinName, hashRate);

        gSheet.Data.CoinsList.AddOrReplase(coin);
    }
}

