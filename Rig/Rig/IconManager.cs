using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ServiceStack;

namespace Rig
{
    class IconManager
    {
        static NotifyIcon notifyIcon = new NotifyIcon();
        static bool Visible = true;
        private static Process myProcess = Process.GetCurrentProcess();
        private static List<Process> cashProcess = new List<Process>();
        [STAThread]
        public void Init()
        {
            myProcess = Process.GetCurrentProcess();
            notifyIcon.Click += ShowBalloonTip;
            notifyIcon.BalloonTipClicked += (s, e) =>
            {
                Visible = !Visible;
                SetConsoleWindowVisibility(Visible);
            };

            //notifyIcon.BalloonTipTitle = "Balloon Tip Title";
            notifyIcon.BalloonTipText = "utility Tip";
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.Visible = true;
            //notifyIcon.Text = MediaTypeNames.Application.ProductName;

//            var contextMenu = new ContextMenuStrip();
//            contextMenu.Items.Add("Exit", null, (s, e) => { MediaTypeNames.Application.Exit(); });
//            notifyIcon.ContextMenuStrip = contextMenu;

            RigEx.WriteLineColors("set icon to tray!".AddTimeStamp(),ConsoleColor.DarkGray);

            // Standard message loop to catch click-events on notify icon
            // Code after this method will be running only after Application.Exit()
            //            MediaTypeNames.Application.Run();
            Application.Run();
        }
        void ShowBalloonTip(object sender, EventArgs eventArgs)
        {
            notifyIcon.ShowBalloonTip(500);

        }

        private static void SetConsoleWindowVisibility(bool visible)
        {
            try
            {
                if (Miner.MinerProcess != null)
                    ShowWindow(Miner.MinerProcess.MainWindowHandle, visible ? 1 : 0);

                if (myProcess != null)
                    ShowWindow(myProcess.MainWindowHandle, visible ? 1 : 0);

                if (visible == false)
                {

                    var processes = Process.GetProcesses().Where(i => i.ProcessName.EndsWith("miner")
                                                                      || i.ProcessName.StartsWith("excavator")
                                                                      //|| i.ProcessName.StartsWith("ccminer")
                                                                      //|| i.ProcessName.StartsWith("Nice")
                                                                      //|| i.ProcessName.StartsWith("ethminer")
                                                                      //|| i.ProcessName.StartsWith("nheqminer")
                                                                      //|| i.ProcessName.StartsWith("sgminer")
                                                                      //|| i.ProcessName.StartsWith("xmrig")
                                                                      || i.ProcessName.StartsWith("xmr")).ToList();
                    if (processes != null)
                    {
                        cashProcess.Clear();
                        processes.ForEach(i =>
                        {
                            cashProcess.Add(i);
                            ShowWindow(i.MainWindowHandle, 0);
                        });
                    }
                }
                else
                {
                    cashProcess.ForEach(i => ShowWindow(i.MainWindowHandle, 1));
                }

//                if (visible)
//                {
//                    //1 = SW_SHOWNORMAL 
//                    ShowWindow(hWnd, 1);
//                    
//
//                }
//                else
//                {
//                    //0 = SW_HIDE 
//                    ShowWindow(hWnd, 0);
//                    if (miner.MinerProcess != null && hWnd != IntPtr.Zero)
//                    {
//                        ShowWindow(miner.MinerProcess.MainWindowHandle, 0);
////                        foreach (ProcessThread thread in miner.MinerProcess.Threads)
////                        {
////                            EnumThreadWindows(thread.Id, new EnumThreadDelegate(ThreadWindows), IntPtr.Zero);
////                        }
//                    }
////                    var processes = Process.GetProcesses().Where(i => i.ProcessName.StartsWith("ccminer")
////                                                  || i.ProcessName.StartsWith("excavator")
////                                                  || i.ProcessName.StartsWith("Nice")
////                                                  || i.ProcessName.StartsWith("ethminer")
////                                                  || i.ProcessName.StartsWith("nheqminer")
////                                                  || i.ProcessName.StartsWith("sgminer")
////                                                  || i.ProcessName.StartsWith("xmrig")
////                                                  || i.ProcessName.StartsWith("xmr-stak-cpu")).ToList();
////                    if (processes != null)
////                    {
////                        for (int i = 0; i < processes.Count(); i++)
////                        {
////                            ShowWindow(processes[i].MainWindowHandle, 0);
////                        }
////                    }
//                }

            }
            catch (Exception e)
            {

                RigEx.WriteLineColors($"Icon Error:{e.Message}".AddTimeStamp(), ConsoleColor.DarkRed);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        private static bool ThreadWindows(IntPtr handle, IntPtr param)
        {
            //get window from handle later, testing for now
            //logger.Info("foo bar");

            return true;
        }

        [STAThread]
        public void Execute(string name )
        {
            Process[] processes = Process.GetProcessesByName("MyProcessName");

            Process processOfInterest = processes[0];

            foreach (ProcessThread thread in processOfInterest.Threads)
            {
                EnumThreadWindows(thread.Id, new EnumThreadDelegate(ThreadWindows), IntPtr.Zero);
            }
        }
    }
}

