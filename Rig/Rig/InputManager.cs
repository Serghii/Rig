using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rig
{
    
    public class InputManager
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOWMINIMIZED = 2;
        const int SW_SHOW = 5;
        const int SW_MINIMIZE = 6;
        private const int SW_HIDE1 = 0x00;
        private const int SW_SHOW1 = 0x05;
        private const int WS_EX_APPWINDOW = 0x40000;
        private const int GWL_EXSTYLE = -0x14;
        private const int WS_EX_TOOLWINDOW = 0x0080;
        private const string AppGuid = "c0a76b5a-12ab-45c5-b9d9-d693faa6e7b9";
        
        public InputManager()
        {
        }
        
//        public void TaskRun()
//        {
//            // thread that listens for keyboard input
//            var kbTask = Task.Run(() =>
//            {
//                while (true)
//                {
//                    var input = Console.ReadKey();
//                    if (input.Modifiers == ConsoleModifiers.Alt && input.Key == ConsoleKey.X)
//                    {
//                        ShowWindow(Program.handle, SW_HIDE);
//                        Program.tray.Visible = true;
//                        //Program.tray.DoubleClick += TrayOnDoubleClick;
//                    }
//                }
//            });
//            kbTask.Wait();
//        }

        
    }
}
