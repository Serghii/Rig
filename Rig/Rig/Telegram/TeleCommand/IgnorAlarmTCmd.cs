using System;

namespace Rig.Telegram.TeleCommand
{
    class IgnorAlarmTCmd : TCommandBase, ITCommand
    {
        public IgnorAlarmTCmd(ITCommandService srv) : base(srv){}

        public TCmdType Type => TCmdType.AlarmIgnor;
        public override string Name => TeleSettings.ignoreAlarm;

        public void Execute()
        {
            Console.WriteLine("Error Ignor Alarm TCmd");
        }

        public void Execute(JsonData jd)
        {
            RigEx.WriteLineColors($"Command Ignor Alarm for {jd.Value}".AddTimeStamp(), ConsoleColor.Yellow);
            srv.Ctrl.AlarmIgnoreCommand(jd.Value);

        }
    }
}
