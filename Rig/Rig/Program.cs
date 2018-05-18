using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rig
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Rig v16 for close print: quit");
            MainClass prog = new MainClass();
            Thread.Sleep(150);
            prog.Run();
            UserInputForExit();
        }
        public static void UserInputForExit()
        {
            var s = Console.ReadLine();
            if (s != "quit")
            {
                UserInputForExit();
            }
            Environment.Exit(0);
        }
    }
}
