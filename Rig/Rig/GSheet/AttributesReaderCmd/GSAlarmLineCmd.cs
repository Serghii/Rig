using System;
using System.Collections.Generic;
using Rig;
using Rig.Telegram;

class GSAlarmLineCmd : AttributesReaderBaseCmd
{
    public GSAlarmLineCmd(IGSheet gSheet) : base(gSheet) { }

    public override string GetAttributesType => GShSettings.alarmLine;
    public override void Execute(IList<object> line)
    {
        if (line[1]?.ToString().ToLower() != GShSettings.PCname)
            return;
        string CpuLoadMin = line.Count > 2 ? line[2]?.ToString().ToLower() : String.Empty;
        string CpuLoadMax = line.Count > 3 ? line[3]?.ToString().ToLower() : String.Empty;

        string CpuTemperatureMin = line.Count > 4 ? line[4]?.ToString().ToLower() : String.Empty;
        string CpuTemperatureMax = line.Count > 5 ? line[5]?.ToString().ToLower() : String.Empty;

        string GpuTemperatureMin = line.Count > 6 ? line[6]?.ToString().ToLower() : String.Empty;
        string GpuTemperatureMax = line.Count > 7 ? line[7]?.ToString().ToLower() : String.Empty;
        AlarmSettings newAlarm;
        if (TryParse(CpuLoadMin, CpuLoadMax, SensorsType.CpuLoad, out newAlarm))
            gSheet.Data.AlarmSettingsList.AddOrReplase(newAlarm);
        if (TryParse(CpuTemperatureMin, CpuTemperatureMax, SensorsType.CpuTemperature, out newAlarm))
            gSheet.Data.AlarmSettingsList.AddOrReplase(newAlarm);
        if (TryParse(GpuTemperatureMin, GpuTemperatureMax, SensorsType.VideoTemperature, out newAlarm))
            gSheet.Data.AlarmSettingsList.AddOrReplase(newAlarm);
    }
}