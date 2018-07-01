using System.Collections.Generic;
using Rig;

class GSMsiAfterburner : AttributesReaderBaseCmd
{
    public GSMsiAfterburner(IGSheet gSheet) : base(gSheet) { }

    public override string GetAttributesType => GShSettings.MsiAfterburner;
    public override void Execute(IList<object> line)
    {
        if (line[1]?.ToString().ToLower() != GShSettings.PCname)
            return;

        int coreMax;
        int.TryParse(line[2].ToString(), out coreMax);
        gSheet.Data.GPUCoreMax = coreMax;
    }
}

