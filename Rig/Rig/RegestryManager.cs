using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Rig
{
    class RegestryManager
    {/*
        private readonly string key = "path";
        private readonly RegistryKey path = Registry.LocalMachine.OpenSubKey(@"Software\Fonya", RegistryKeyPermissionCheck.ReadWriteSubTree);
        private RegistryKey skl ;
        public void SetPathToRegistry()
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey skl = rk.CreateSubKey(@"Software\Fonya",RegistryKeyPermissionCheck.ReadWriteSubTree);
                skl.SetValue(key, RigEx.PathFull);
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Error Write to registry: {e.Message}".AddTimeStamp(),ConsoleColor.DarkRed);
            }
        }

        public string ReadKey()
        {
            string kq = String.Empty;
            try
            {
//                RegistryKey rk = Registry.LocalMachine;
//                RegistryKey skl = rk.CreateSubKey(@"Software\Fonya");
                if (path == null)
                {
                    Console.WriteLine("no registry path");
                    return kq;
                }
                Console.WriteLine(path.Name);
                kq = (string)path.GetValue(key);
            }
            catch (Exception e)
            {
                RigEx.WriteLineColors($"Error Read registry: {e.Message}".AddTimeStamp(), ConsoleColor.DarkRed);
            }
            return kq;
        }
    */}
}
