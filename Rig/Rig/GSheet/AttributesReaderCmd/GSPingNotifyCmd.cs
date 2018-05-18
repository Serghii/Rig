using System;
using System.Collections.Generic;
using Rig;


class GSPingNotifyCmd : AttributesReaderBaseCmd
{
    public GSPingNotifyCmd(IGSheet gSheet) : base(gSheet) { }

    public override string GetAttributesType => GShSettings.PingNotify;
    public override void Execute(IList<object> line)
    {
        if (line.Count < 2)
        {
            gSheet.Data.PingDelayMillisec = 240000; // 4 min
            return;
        }
        int minutes = 4;
        if (!int.TryParse(line[1]?.ToString(), out minutes))
        {
            RigEx.WriteLineColors("ping alarm: can not parse to int from B colum ", ConsoleColor.Red);
        }
        minutes = minutes == 0 ? 4 : minutes;
        gSheet.Data.PingDelayMillisec = minutes * 60000;
    }
}