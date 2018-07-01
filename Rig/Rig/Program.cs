using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;


namespace Rig
{
    class Program
    {
        static void Main()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyTitleAttribute assemblyTitle = assembly?.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute;
            Console.Title = assemblyTitle?.Title;
            StopDuplicateApp();
            new VersionManager();
            WriteStartFile();
            RigEx.WriteLineColors($"cur version:{RigEx.MyVersion} for close prees: q".AddTimeStamp(),ConsoleColor.Gray);

            MainClass prog = new MainClass();
            Thread.Sleep(150);
            prog.Run();
            UserInputForExit();
        }

        public static void UserInputForExit()
        {
            while (Console.ReadKey().Key != ConsoleKey.Q)
            {
//                if (this.WindowState == FormWindowState.Minimized)
//                {
            }
            RigEx.WriteLineColors("YOU PRESS q APP WILL BE QUIT IN 30 sek".AddTimeStamp(),ConsoleColor.Red);
            
            RigEx.QuitApp(30000);
        }

        private static void StopDuplicateApp()
        {
            Process process = Process.GetCurrentProcess();
            var dupl = Process.GetProcessesByName(process.ProcessName);
            if (dupl.Length > 1)
            {
                foreach (var p in dupl)
                {
                    if (p.Id != process.Id)
                    {
                        p.CloseMainWindow();
                        p?.Close();
                    }
                }
            }
        }

        public static void WriteStartFile()
        {
            
            var path = Path.Combine(RigEx.MainFolderPath, "start.bat");
            if (File.Exists(path))
            {
                File.Delete(path);
                using (StreamWriter w = new StreamWriter(path))
                {
                    w.WriteLine("@echo off");
                    w.WriteLine("timeout /t 5 /nobreak");
                    w.WriteLine($"start \"\" \"{RigEx.PathFull}\"");
                    w.WriteLine("timeout /t 5 /nobreak");
                    w.Close();
                }
                RigEx.WriteLineColors($"edit: start.bat ".AddTimeStamp(), ConsoleColor.DarkGray);
            }
            else
            {
                RigEx.WriteLineColors($"Error: start.bat put to =>  {RigEx.MainFolderPath}".AddTimeStamp(),ConsoleColor.Red);
                Console.ReadLine();
            }
            
        }
        [DllImport("user32.dll")]
        internal static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);
        static Int32 WM_SYSCOMMAND = 0x0112;
        static Int32 SC_MINIMIZE = 0x0F020;
    }
}
