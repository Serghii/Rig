using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rig.Telegram;

namespace Rig
{
    public class Miner
    {
        private static Process minerProcess;
        private static ProcessStartInfo startInfo = new ProcessStartInfo();
        private static Thread minerThread;
        private IMiningCtrl ctrl;
        public bool IsActive => minerProcess != null && !minerProcess.HasExited && minerProcess.Responding;

        public static Process MinerProcess
        {
            get { return minerProcess; }
        }

        public Miner(IMiningCtrl minerCtrl)
        {
            ctrl = minerCtrl;
            var minerName = RigEx.Read(RigEx.Lastminer);
            RigEx.WriteLineColors($"Last miner is: {minerName}".AddTimeStamp(),ConsoleColor.DarkCyan);
            ChangeAndRunMinerAction(minerName);
            ctrl.ButtonChangeAndRunMinerAction += ChangeAndRunMinerAction;
            ctrl.MinerActivityAction += OnMinerActivityAction;
        }

        private async void OnMinerActivityAction()
        {
            if (ctrl.MinerStatus != IsActive)
            {
                if (ctrl.MinerStatus)
                {
                    RigEx.WriteLineColors("Launch miner ".AddTimeStamp(), ConsoleColor.DarkCyan);
                    await ChangeAndRunMinerAction(ctrl.CurMiner.Name);
                }
                else
                {
                    RigEx.WriteLineColors("Stop miner ".AddTimeStamp(), ConsoleColor.DarkCyan);
                    Destroy();
                }
            }
        }

        public async void ChangeAndRunMinerAction(IEventArgs minerName)
        {
            if (minerName != null && !string.IsNullOrEmpty(minerName.Value))
                await ChangeAndRunMinerAction(minerName.Value);
            else
                RigEx.WriteLineColors("miner ChangeMiner => miner is null ", ConsoleColor.DarkRed);
        }

        public async Task ChangeAndRunMinerAction(string name)
        {
            var newminer = ctrl.Miners.FirstOrDefault(i => i.Name == name);
            
            if (newminer == null)
            {
                RigEx.WriteLineColors($"Cannot find: {name}".AddTimeStamp(), ConsoleColor.DarkRed);
                newminer = ctrl?.Miners?.First();
            }
            if (ctrl.CurMiner == newminer && IsActive)
            {
                Console.WriteLine($"Already Started {ctrl.CurMiner.Name}");
                return;
            }
            await Destroy();
            ctrl.CurMiner = newminer;
            RigEx.WriteLineColors($"Start: {newminer.Name}".AddTimeStamp(), ConsoleColor.DarkCyan);
            ctrl.SendMsg($"Start miner: {newminer.Name}");
            LaunchCommandLine();
        }

        private void LaunchCommandLine(IEventArgs minerName = null)
        {
            if (ctrl?.CurMiner == null)
            {
                RigEx.WriteLineColors("LaunchMSI : current miner is null".AddTimeStamp(), ConsoleColor.Red);
                return;
            }
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;
            startInfo.FileName = ctrl.CurMiner.Path;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            LaunchMiner();
        }

        public void LaunchMiner()
        {
            if (!ctrl.MinerStatus)
                return;

            if (!IsActive)
            {

                minerThread = new Thread(Start) {IsBackground = true};
                minerThread.Start();
            }
            else
            {
                Console.WriteLine("Already Started");
            }
        }

        private async void Start()
        {
            try
            {
                minerProcess = Process.Start(startInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error launch {startInfo.FileName}: {e.Message} ");
            }
            while (true)
            {
                Thread.Sleep(1000);
                if (ctrl.MinerStatus && minerProcess.HasExited)
                {
                    RigEx.WriteLineColors($"relaunching: {ctrl.CurMiner.Name}".AddTimeStamp(), ConsoleColor.DarkCyan);
                    LaunchMiner();
                    return;
                }
            }
        }

        public async Task AppExit()
        {
            await Destroy();
        }

        public async Task Destroy()
        {
            if (minerThread != null)
            {
                try
                {
                    minerThread.Abort(startInfo);
                    minerThread.DisableComObjectEagerCleanup();
                    minerThread = null;

                }
                catch (Exception e)
                {
                    RigEx.WriteLineColors($"Destroy miner Thread: cannot Abort Thread {e.Message}".AddTimeStamp(),
                        ConsoleColor.DarkRed);
                }

            }
            if (minerProcess != null)
            {
                try
                {
                    minerProcess.CloseMainWindow();
                    minerProcess.Kill();
                    minerThread = null;

                }
                catch (Exception e)
                {
                    RigEx.WriteLineColors($"Destroy miner Process: cannot kill process {e.Message}".AddTimeStamp(),
                        ConsoleColor.DarkRed);
                }
               
            }
            KillMiners();
        }

        public void KillMiners()
        {
            var processes = Process.GetProcesses().Where(i => i.ProcessName.StartsWith("Nice")
                                                              || i.ProcessName.Contains("miner")
                                                              || i.ProcessName.Contains("Miner")
                                                              || i.ProcessName.StartsWith("zm")
//                                                              || i.ProcessName.EndsWith("iner64")
                                                              || i.ProcessName.StartsWith("excavator")
//                                                              || i.ProcessName.StartsWith("ETC")
//                                                              || i.ProcessName.StartsWith("ETH")
//                                                              || i.ProcessName.StartsWith("XMR")
//                                                              || i.ProcessName.StartsWith("Eth")
                                                              /*|| i.ProcessName.StartsWith("sgminer")
                                                              || i.ProcessName.StartsWith("ethminer")
                                                              || i.ProcessName.StartsWith("nheqminer")
                                                              || i.ProcessName.StartsWith("NhEqMiner")
                                                              || i.ProcessName.StartsWith("NsGpuCNMiner")
                                                              || i.ProcessName.StartsWith("ZecMiner64")
                                                              || i.ProcessName.StartsWith("EthDcrMiner64")
                                                              || i.ProcessName.StartsWith("sgminer")*/
                                                              || i.ProcessName.StartsWith("prospector")
                                                              || i.ProcessName.Contains("Xmr")
                                                              || i.ProcessName.Contains("xmr")).ToArray();
            if (processes != null)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    try
                    {
                        RigEx.WriteLineColors($"kill process {processes[i]?.ProcessName})".AddTimeStamp(),ConsoleColor.DarkCyan);
                        processes[i]?.CloseMainWindow();
                    }
                    catch{}

                    try
                    {
                        processes[i]?.Kill();
                    }
                    catch{}
                }
            }
        }
    }

    
}
