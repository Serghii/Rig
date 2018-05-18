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

        public Miner(IMiningCtrl minerCtrl)
        {
            ctrl = minerCtrl;
            var minerName = RigEx.Read(RigEx.Lastminer);
            RigEx.WriteLineColors($"Last miner is: {minerName}".AddTimeStamp(),ConsoleColor.DarkCyan);
            ChangeAndRunMinerAction(minerName);
            ctrl.ButtonChangeAndRunMinerAction += ChangeAndRunMinerAction;
            ctrl.MinerActivityAction += OnMinerActivityAction;
        }

        private void OnMinerActivityAction()
        {
            if (ctrl.MinerStatus != IsActive)
            {
                if (ctrl.MinerStatus)
                {
                    RigEx.WriteLineColors("Launch miner ".AddTimeStamp(), ConsoleColor.DarkCyan);
                    LaunchCommandLine();
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
                RigEx.WriteLineColors($"Cannot find miner: {name}".AddTimeStamp(), ConsoleColor.Red);
                newminer = ctrl?.Miners?.First();
            }
            if (ctrl.CurMiner == newminer && IsActive)
            {
                Console.WriteLine($"miner Already Started {ctrl.CurMiner.Name}");
                return;
            }
            await Destroy();
            ctrl.CurMiner = newminer;
            RigEx.WriteLineColors($"Start miner: {newminer.Name}".AddTimeStamp(), ConsoleColor.DarkCyan);
            ctrl.SendMsg($"Start miner: {newminer.Name}");
            LaunchCommandLine();
        }

        private void LaunchCommandLine(IEventArgs minerName = null)
        {
            if (ctrl?.CurMiner == null)
            {
                RigEx.WriteLineColors("LaunchMiner : current miner is null".AddTimeStamp(), ConsoleColor.Red);
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

        private void Start()
        {
            try
            {
                minerProcess = Process.Start(startInfo);
//                minerProcess.WaitForExit();
//                if (ctrl.MinerStatus)
//                {
//                    RigEx.WriteLineColors("WaitForExit ".AddTimeStamp(), ConsoleColor.DarkCyan);
//                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error lounch {startInfo.FileName}: {e.Message} ");
            }
            while (true)
            {
                Thread.Sleep(1000);
                if (ctrl.MinerStatus && minerProcess.HasExited)
                {
                    RigEx.WriteLineColors($"relaunching miner: {ctrl.CurMiner.Name}".AddTimeStamp(), ConsoleColor.DarkCyan);
                    LaunchMiner();
                    return;
                }
            }
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
                    minerProcess.Kill();
                    minerThread = null;

                }
                catch (Exception e)
                {
                    RigEx.WriteLineColors($"Destroy miner Process: cannot kill process {e.Message}".AddTimeStamp(),
                        ConsoleColor.DarkRed);
                }

                var processes = Process.GetProcesses().ToList();
                var cc = processes.Where(i => i.ProcessName.StartsWith("ccminer")
                                              || i.ProcessName.StartsWith("excavator")
                                              || i.ProcessName.StartsWith("ethminer")
                                              || i.ProcessName.StartsWith("nheqminer")
                                              || i.ProcessName.StartsWith("sgminer")
                                              || i.ProcessName.StartsWith("xmrig")
                                              || i.ProcessName.StartsWith("xmr-stak-cpu")).ToList();
                if (cc != null)
                {
                    for (int i = 0; i < cc.Count(); i++)
                    {
                        RigEx.WriteLineColors($"miner: {cc[i].ProcessName}", ConsoleColor.Cyan);
                        cc[i].Kill();
                    }
                }

            }
        }

        public void AppExit()
        {
            
            Destroy();
        }
    }

    
}
