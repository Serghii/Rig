using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Rig;
using File = System.IO.File;
using Timer = System.Timers.Timer;


public static class RigEx
{
    private static  readonly string fullpath = Path.GetFullPath(Assembly.GetEntryAssembly().Location);
    private static  readonly string mainFolderPath = Path.GetFullPath(Path.Combine(fullpath, @"..\..\"));
    public static readonly string Lastminer = "lastMiner.txt";
    public static readonly string nice = "nice.txt";
    public static readonly Version MyVersion = Assembly.GetEntryAssembly().GetName().Version;
    public static string AddTimeStamp(this string s)
    {
        return $"[{DateTime.Now:MM_dd HH:mm:ss tt}] : {s}";
    }

    public static string MainFolderPath => mainFolderPath;
    public static string PathFull => fullpath;

    public static void WriteLineColors(string msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void AddifNew<T>(this IList<T> l,T el)
    {
        if (l != null && !l.Contains(el))
        {
            l.Add(el);
        }
    }
    public static TimeSpan ToTimeSpan(int minutes)
    {
        float d = minutes/1440f;
        float h = (d - (int) d)*24;
        float m = minutes%1440f%60f;
        return new TimeSpan((int) d, (int) h, (int) m, 0);
    }

    public static void Delayed(int milliseconds, Action action)
    {
        Timer timer = new Timer();
        timer.Interval = milliseconds;
        timer.Elapsed += (s, e) => {
            action();
            timer.Stop();
        };
        timer.Start();
    }

    public static int LaterToColum(this string s)
    {
        return Int32.Parse(s.ToUpper(), NumberStyles.HexNumber) - 10;
    }

    public static void WriteLog(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss tt}] : {message}");
    }

    public static IEnumerable<I> As<T, I>(this List<T> list) where T : I
    {
        return list.Cast<I>();
    }

    public static void AddOrReplase<T>(this List<T> list, T obj) where T : class 
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(obj))
            {
                list[i] = obj;
                return;
            }
        }
        list.Add(obj);
    }
    public static void SafeLastMiner(string minerName)
    {
        Safe(minerName,Lastminer);
    }
    public static void Safe(string msg, string fileName, string path = null)
    {
        var Path = System.IO.Path.Combine((path ?? RigEx.MainFolderPath) , fileName);
             File.WriteAllText(Path, msg, Encoding.UTF8);
    }

    public static string Read(string fileName, string path = null)
    {
        var Path = System.IO.Path.Combine((path ?? RigEx.MainFolderPath) , fileName);
        if (File.Exists(Path))
            return File.ReadAllText(Path);

        WriteLineColors($"Error cannot find file: {fileName}".AddTimeStamp(),ConsoleColor.DarkRed);
        return String.Empty;
    }
    public static void QuitApp(int delayMillisecond)
    {
        var aTimer = new Timer(delayMillisecond);
        aTimer.Elapsed += (t, e) => { Environment.Exit(0); };
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }

    #region NiceHash
    public static NiceHashKey GetNiceHashKey()
    {
        string json = Read(nice);
        return json == null ? null : JsonConvert.DeserializeObject<NiceHashKey>(json);
        
    }
//    public static void KillMiners()
//    {
//        var processes = Process.GetProcesses().Where(i => i.ProcessName.StartsWith("Nice")
//                                      || i.ProcessName.StartsWith("excavator")
//                                      || i.ProcessName.EndsWith("miner")
//                                      || i.ProcessName.StartsWith("xmr")).ToList();
//        if (processes != null)
//        {
//            for (int i = 0; i < processes.Count(); i++)
//            {
//                processes[i].CloseMainWindow();
//                processes[i].Kill();
//            }
//        }
//        Console.WriteLine(processes?.Count);
//    }

    public class NiceHashKey
    {
        public string id { get; set; }
        public string keyRead { get; set; }
    }
    #endregion
}

