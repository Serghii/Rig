using System;
using System.Collections.Generic;
using Rig;


class GSMinerLineCmd : AttributesReaderBaseCmd
{
    public GSMinerLineCmd(IGSheet gSheet) : base(gSheet) { }

    public override string GetAttributesType => GShSettings.minerLine;
    public override void Execute(IList<object> line)
    {
        if (line[1] == null || line[1].ToString() == string.Empty)
            RigEx.WriteLineColors("Google Sheet cannot read miner name from  B".AddTimeStamp(), ConsoleColor.Red);
        //gSheet.Data.MinersList.Add(new MinerInfo(line[1].ToString(), line[2].ToString()));
    }
}