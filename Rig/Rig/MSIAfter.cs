using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Rig.Telegram;

namespace Rig
{
    class MSIAfter
    {
        private ICtrlMsi ctrl;
        private static ProcessStartInfo startInfo = new ProcessStartInfo();
        private List<ISensorProperty> sensors = new List<ISensorProperty>();
        private static bool isLocked = false;

        public MSIAfter(ICtrlMsi data)
        {
            string path = GetShortCutPath();
            if (string.IsNullOrEmpty(path))
                return;

            ctrl = data;
            foreach (var s in ctrl.GetSensors.GetSensor)
            {
                AddNewAlarmSensor(s);
            }
            ctrl.GetSensors.AddSensorAction += AddNewAlarmSensor;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;
            startInfo.FileName = path;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            ReLaunchMSI();
        }

        private void AddNewAlarmSensor(ISensorProperty s)
        {
            if (s.Dictionary.ContainsKey(SensorsType.GPUCore))
            {
                RigEx.WriteLineColors($"MSI sensor detected: {s.Name} =>   {s.Dictionary.Keys.Last().ToString()}".AddTimeStamp(), ConsoleColor.DarkGray);
                sensors.AddifNew(s);
                s.SensorAction += OnSensorAction;
            }
        }

        private void OnSensorAction(SensorsType sensorsType)
        {
            if (isLocked)
            {
                RigEx.WriteLineColors("MSI isLocked".AddTimeStamp(),ConsoleColor.DarkBlue);
                return;
            }

            foreach (var sensor in sensors)
            {
                var gpuCore = sensor.Dictionary[sensorsType];
                if (ctrl.GPUCoreMax > 0 && gpuCore > ctrl.GPUCoreMax)
                {
                    RigEx.WriteLineColors($"core:{gpuCore} -> ReLaunchMSI".AddTimeStamp(), ConsoleColor.DarkBlue);
                    isLocked = true;
                    ReLaunchMSI();
                    return;
                }
            }
        }

        private void ReLaunchMSI()
        {
            if (string.IsNullOrEmpty(startInfo.FileName))
            {
                RigEx.WriteLineColors($"not found msiafterburn path ".AddTimeStamp(),ConsoleColor.DarkRed);
                return;
            }
            if (!TryKillMSI())
            {
                RigEx.WriteLineColors($"can not kill msiAfterburn process ".AddTimeStamp(), ConsoleColor.DarkRed);
                return;
            }
            try
            {
                RigEx.Delayed(1500, ()=>
                {
                    Process.Start(startInfo);
                    RigEx.WriteLineColors($"start msiAfterburn".AddTimeStamp(), ConsoleColor.DarkBlue);
                });
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error launch {startInfo.FileName}: {e.Message} ");
            }
            isLocked = false;
        }

        private bool TryKillMSI()
        {
            var processes = Process.GetProcesses().Where(i => i.ProcessName.StartsWith("MSIAfterburn")).ToArray();
            if (processes != null)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    try
                    {
                        RigEx.WriteLineColors($"kill process {processes[i]?.ProcessName})".AddTimeStamp(), ConsoleColor.DarkBlue);
                        processes[i]?.CloseMainWindow();
                        processes[i]?.Close();
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private string GetShortCutPath()
        {
            var shortcutPath = System.IO.Directory
                .GetFiles(RigEx.MainFolderPath, "*.lnk", SearchOption.TopDirectoryOnly)
                .FirstOrDefault(i => i.EndsWith("Afterburner.lnk"));
            if (shortcutPath == null || !shortcutPath.Any())
            {
                RigEx.WriteLineColors(
                    $"not found msiafterburn shortcut in\n\t\t\t{RigEx.MainFolderPath}".AddTimeStamp(),
                    ConsoleColor.Gray);
                return String.Empty;
            }
            return shortcutPath;

        }

        private string GetFileName(string documentPath)
        {
            return Path.GetFileNameWithoutExtension(documentPath);
        }
    }
}
