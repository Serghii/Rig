using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rig.Telegram
{

    public interface IAlarmSettings
    {
        SensorsType sensorType { get; }
        int Low { get; }
        int Hight { get; }

    }
    public class AlarmSettings : IAlarmSettings
    {
        
        public AlarmSettings(SensorsType sensorType, int low, int hight)
        {
            this.sensorType = sensorType;
            Low = low;
            Hight = hight;
        }

        public SensorsType sensorType { get; private set; }
        public int Low { get; private set; }
        public int Hight { get; private set; }
        public override bool Equals(object obj)
        {
            var c = obj as IAlarmSettings;
            return sensorType == c?.sensorType;
        }
    }
    public enum SensorsType
    {
        none,
        CpuLoad,
        CpuTemperature,
        VideoTemperature,
        GPUCore
    }

    public enum AlarmType
    {
        Low,
        High
    }

    public interface IAlarmSensor
    {
       
        AlarmType AlarmType { get; }
        SensorsType SensorType { get; }
        ISensorProperty Sensor { get; }
    }

    public class AlarmSensor: IAlarmSensor
    {
        public Action<IAlarmSensor> AlarmAction = (AlarmObject) => { };
        public Action<IAlarmSensor> StopMiningAction = (AlarmObject) => { };
        public float cooldownSeconds = 0.05f;
        public DateTime cooldownAllarmTime = DateTime.Now;
        public ISensorProperty sensor;
        private ISensorsSetings ctrl;
        private readonly int LowAlarmMax = 3;
        private int lowAlarmCoutner = 0;
        private DateTime timer = DateTime.Now;
        public AlarmSensor(ISensorProperty sensor, ISensorsSetings settings)
        {
            ctrl = settings;
            this.sensor = sensor;
            
            sensor.SensorAction += OnSensorAction;
        }

        private void OnSensorAction(SensorsType sensorsType)
        {
            //stop miner if need
            var stopSeting = ctrl.GetStopSettings.FirstOrDefault(i => i.sensorType == sensorsType);

            if (stopSeting != null)
            {
                if (sensor.Dictionary[sensorsType] <= stopSeting.Low)
                {
                    CallAlarm(AlarmType.Low, sensorsType, StopMiningAction);
                }
                else if (sensor.Dictionary[sensorsType] >= stopSeting.Hight)
                    CallAlarm(AlarmType.High, sensorsType, StopMiningAction);
            }
            // allarm if need
            if (cooldownAllarmTime > DateTime.Now && !ctrl.SensorActivity[sensorsType])
                return;
            
            var alarmSeting = ctrl.GetAlarmSettings.FirstOrDefault(i => i.sensorType == sensorsType);
            if (alarmSeting == null)
                return;

            if (sensor.Dictionary[sensorsType] >= alarmSeting.Hight)
                    CallAlarm(AlarmType.High, sensorsType, AlarmAction);

            if (sensor.Dictionary[sensorsType] <= alarmSeting.Low)
            {
                if (DateTime.Now < timer)
                    return;

                timer = DateTime.Now.AddSeconds(15);

                if (lowAlarmCoutner >= LowAlarmMax)
                {
                    lowAlarmCoutner = 0;
                    CallAlarm(AlarmType.Low, sensorsType, AlarmAction);
                }
                else
                {
                    lowAlarmCoutner++;
                    if (lowAlarmCoutner > 1)
                        RigEx.WriteLineColors(
                        $"Alarm => [{lowAlarmCoutner}/{LowAlarmMax} / {ctrl.GetAlarmSettings.Count()}] cooldown: 15sec\tsensor: [ {sensor.Name} ] => [ {sensorsType} ]: [ {sensor.Dictionary[sensorsType]} ] - [ low ]"
                            .AddTimeStamp(), ConsoleColor.DarkMagenta);
                }
            }
            else
            {
                timer = DateTime.Now;
                lowAlarmCoutner = 0;
            }
        }

        public ISensorProperty Sensor => sensor;
        
        public AlarmType AlarmType { get; private set; }
        public SensorsType SensorType { get; private set; }
        
        private void CallAlarm(AlarmType aType, SensorsType sType, Action<IAlarmSensor> action)
        {
            AlarmType = aType;
            SensorType = sType;
            cooldownAllarmTime = DateTime.Now.AddSeconds(cooldownSeconds);
            action(this);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (this == null || obj == null)
                return false;
            AlarmSensor c = obj as AlarmSensor;
            return sensor.Name == c?.sensor.Name
                   && sensor.HwType == c.sensor.HwType
                   && sensor.Dictionary.Count == c.sensor.Dictionary.Count && !sensor.Dictionary.Except(c.sensor.Dictionary).Any();
            return base.Equals(obj);
        }
    }

    public class AlarmData
    {
        public static Action<IAlarmSensor> AlarmAnyAction = s =>{};

        private ICtrlAlarm ctrl;
        public AlarmData(ICtrlAlarm Alarmdata)
        {
            ctrl = Alarmdata;
            foreach (var s in ctrl.GetSensors.GetSensor)
            {
                AddNewAlarmSensor(s);
            }
            ctrl.GetSensors.AddSensorAction += AddNewAlarmSensor;
        }

        private void AddNewAlarmSensor(ISensorProperty s)
        {
           RigEx.WriteLineColors($"sensor detected: {s.Name} =>   {s.Dictionary.Keys.Last().ToString()}".AddTimeStamp(),ConsoleColor.DarkGray);
            var alarmsensor = new AlarmSensor(s, ctrl);
            alarmsensor.AlarmAction += OnAlarmAction;
            alarmsensor.StopMiningAction += OnStopMiningAction;
            ctrl.GetAlarmdata.AddifNew(alarmsensor);
        }

        private void OnStopMiningAction(IAlarmSensor sensor)
        {
            if (ctrl.MinerStatus && sensor.AlarmType == AlarmType.High )
            {
                RigEx.WriteLineColors($"Stop miner {sensor.Sensor.Name}=> {sensor.SensorType} :{sensor.Sensor.Dictionary[sensor.SensorType]} - {sensor.AlarmType}".AddTimeStamp(), ConsoleColor.Magenta);
                ctrl.SetMinigActivityStatus(false);
            }
            else if(!ctrl.MinerStatus && sensor.AlarmType == AlarmType.Low)
            {
                RigEx.WriteLineColors($"Start miner {sensor.Sensor.Name}=> {sensor.SensorType} :{sensor.Sensor.Dictionary[sensor.SensorType]} - {sensor.AlarmType}".AddTimeStamp(), ConsoleColor.Magenta);
                ctrl.SetMinigActivityStatus(true);
            }
        }

        private void OnAlarmAction(IAlarmSensor sensor)
        {
            RigEx.WriteLineColors($"Alarm {sensor.Sensor.Name}=> {sensor.SensorType} :{sensor.Sensor.Dictionary[sensor.SensorType]} - {sensor.AlarmType}".AddTimeStamp(), ConsoleColor.Magenta);
            if (ctrl.SensorActivity.ContainsKey(sensor.SensorType))
            {
                if (ctrl.SensorActivity[sensor.SensorType])
                {
                    ctrl.CallAlarm(sensor);
                }
                else
                {
                    RigEx.WriteLineColors($"Alarm: {sensor.SensorType} - is not active by user".AddTimeStamp(), ConsoleColor.DarkMagenta);
                }
            }
            else
            {
                RigEx.WriteLineColors($"Alarm: {sensor.SensorType} - can not find in Sensor Activity list".AddTimeStamp(), ConsoleColor.DarkMagenta);
                ctrl.CallAlarm(sensor);
            }
        }
    }

}
