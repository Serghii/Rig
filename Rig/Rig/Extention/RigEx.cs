using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using File = System.IO.File;


public static class RigEx
{
    private static  readonly string path = System.IO.Path.GetFullPath(System.IO.Path.Combine(Assembly.GetEntryAssembly().Location, @"..\..\"));
    public static readonly string Lastminer = "lastMiner.txt";
    public static string AddTimeStamp(this string s)
    {
        return $"[ {DateTime.Now:HH:mm:ss tt}] : {s}";
    }

    public static string Path => path;

    public static void WriteLineColors(string msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void AddifNew<T>(this IList<T> l,T el)
    {
        if (!l.Contains(el))
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
        var Path = System.IO.Path.Combine((path ?? RigEx.Path) , fileName);
             File.WriteAllText(Path, msg, Encoding.UTF8);
    }
    public static string Read(string fileName, string path = null)
    {
        var Path = System.IO.Path.Combine((path ?? RigEx.Path) , fileName);
        if (File.Exists(Path))
            return File.ReadAllText(Path);
        return String.Empty;
    }

    
    public static void QuitApp(int delayMillisecond)
    {
        var aTimer = new System.Timers.Timer(delayMillisecond);
        aTimer.Elapsed += (t, e) => { Environment.Exit(0); };
        aTimer.AutoReset = true;
        aTimer.Enabled = true;

    }
   
}

