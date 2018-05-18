using System.Collections.Generic;
using Rig;
using Rig.Telegram;

public interface IAttributesReader
{
    string GetAttributesType { get; }
    void Execute(IList<object> line);
}

public abstract class AttributesReaderBaseCmd: IAttributesReader
{
    protected IGSheet gSheet;
    public AttributesReaderBaseCmd(IGSheet gSheet)
    {
        this.gSheet = gSheet;
    }

    public abstract string GetAttributesType { get; }
    public abstract void Execute(IList<object> line);

    protected bool TryParse(string min, string max, SensorsType type, out AlarmSettings alarm)
    {
        int low, high;
        bool lowParse, highParse;
        lowParse = int.TryParse(min, out low);
        highParse = int.TryParse(max, out high);

        if (!highParse)
            high = int.MaxValue;
        if (!lowParse)
            low = int.MinValue;
        if (lowParse || highParse)
        {
            alarm = new AlarmSettings(type, low, high);
            return true;
        }
        alarm = null;
        return false;
    }
}

