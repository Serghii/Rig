using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Rig;
using Rig.Telegram;
using Timer = System.Timers.Timer;

public interface ISensorProperty
{
    event Action<SensorsType> SensorAction;
    string Name { get; }
    HWType HwType { get; }
    Dictionary<SensorsType, float> Dictionary { get; }
}
public class SensorR: ISensorProperty
{
    public event Action<SensorsType> SensorAction = type => { };
    private Dictionary<SensorsType, float> dictionary = new Dictionary<SensorsType, float>();
    public string Name { get; private set; }
    public HWType HwType { get; private set; }

    public SensorR(string name, SensorsType type, HWType hwType, float value)
    {
        Name = name;
        HwType = hwType;
        TryChange(type,value);
    }

    public Dictionary<SensorsType, float> Dictionary => dictionary;


    public bool TryChange(SensorsType type, float value)
    {
        if (!dictionary.ContainsKey(type))
        {
            dictionary.Add(type,value);
            //SensorAction(type);
            return true;
        }
        else if(Math.Abs(dictionary[type]-value) > 0.1f)
        {
            dictionary[type] = value;
            SensorAction(type);
            return true;
        }
        return false;
    }
    public override bool Equals(object obj)
    {
        if (this == obj)
            return true;
        if (this == null || obj == null)
            return false;
        ISensorProperty c = obj as ISensorProperty;
        return Name == c?.Name 
               && HwType == c.HwType
               && dictionary.Count == c.Dictionary.Count && !dictionary.Except(c.Dictionary).Any();
    }
       
}

namespace Rig
{
    public enum HWType
    {
        cpu,
        gpu
    }

    public interface ISensors
    {
        event Action<ISensorProperty> AddSensorAction;
        IEnumerable<ISensorProperty> GetSensor { get; }
        void Add(string Name, HWType HwType, SensorsType Type, float Value);
    }
    public class Sensors : ISensors
    {
        public event Action<ISensorProperty> AddSensorAction = p => {};
        //public Enumerable<ISensorProperty> list => sensors.As<SensorR, ISensorProperty>();

        private List<SensorR> sensors = new List<SensorR>();
        public IEnumerable<ISensorProperty> GetSensor => sensors.As<SensorR, ISensorProperty>();

        public void Add(string Name, HWType HwType, SensorsType Type, float Value)
        {
            var c = sensors.FirstOrDefault(i => i.Name == Name && i.Dictionary.ContainsKey(Type) );
            if (c == null)
            {
                var s = new SensorR(Name, Type, HwType, Value);
                sensors.Add(s);
                AddSensorAction(s);
                return;
            }
            else
            {
                c.TryChange(Type, Value);
            }
        }
    }

    class SensorService
    {

        private Thread SensorThread;
        private readonly Computer myComputer = new Computer();
        private ICtrlSensorServise ctrl;
        private Timer SensorTimer;
        public SensorService(ICtrlSensorServise controller)
        {
            ctrl = controller;
            StartUpdateSensors();
        }

        public void Destroy()
        {
            SensorThread.Abort();
            SensorThread = null;
        }

        private void StartUpdateSensors()
        {

            myComputer.CPUEnabled = true;
            myComputer.GPUEnabled = true;
            
            myComputer.Open();
            RunSensorUpdate();
//            SensorThread = new Thread(Update);
//            SensorThread.IsBackground = true;
//            SensorThread.Start();
        }
        private void RunSensorUpdate()
        {
            if (SensorTimer != null)
            {
                SensorTimer.Elapsed -= OnSensorTimerEvent;
            }

            SensorTimer = new System.Timers.Timer(2000);
            SensorTimer.Elapsed += OnSensorTimerEvent;
            SensorTimer.AutoReset = true;
            SensorTimer.Enabled = true;
        }

        private void OnSensorTimerEvent(object sender, ElapsedEventArgs e)
        {
            if (myComputer == null)
                return;

                foreach (var hardwareItem in myComputer.Hardware)
                {
                    hardwareItem.Update();
                    hardwareItem.GetReport();
                    UpdateAllSensors(hardwareItem.Sensors);
                }
        }

        private void Update()
        {
            if (myComputer == null)
                return;

            while (true)
            {
                Thread.Sleep(1000);
                foreach (var hardwareItem in myComputer.Hardware)
                {
                    hardwareItem.Update();
                    hardwareItem.GetReport();
                    UpdateAllSensors(hardwareItem.Sensors);
                }
            }
        }
        private void UpdateAllSensors(ISensor[] Sensors)
        {
            float newCPULoad = -1;
            foreach (ISensor sensor in Sensors)
            {
                if (sensor?.Value == null)
                    return;
                if (sensor?.Hardware.HardwareType == HardwareType.CPU )
                {
                    if (sensor.Name == "CPU Total")
                        newCPULoad = sensor.Value.Value;
                    else if (sensor.SensorType == SensorType.Temperature)
                    {
                        ctrl.GetSensors.Add(sensor.Hardware.Identifier.ToString(), HWType.cpu, SensorsType.CpuTemperature, sensor.Value.Value);
                        ctrl.GetSensors.Add(sensor.Hardware.Identifier.ToString(), HWType.cpu, SensorsType.CpuLoad, newCPULoad);
                    }
                    continue;
                }

                if (sensor.Hardware?.HardwareType == HardwareType.GpuAti
                    || sensor.Hardware?.HardwareType == HardwareType.GpuNvidia)

                {
                    if (sensor.SensorType == SensorType.Clock && sensor.Name == "GPU Core")
                        ctrl.GetSensors.Add(sensor.Hardware.Identifier.ToString(), HWType.gpu, SensorsType.GPUCore, sensor.Value.Value);

                    else if (sensor.SensorType == SensorType.Temperature)
                        ctrl.GetSensors.Add(sensor.Hardware.Identifier.ToString(),HWType.gpu, SensorsType.VideoTemperature, sensor.Value.Value);
                    continue;
                }
            }
        }
    }
}
