using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NanoShop.PayPal
{
    public class PayPalLogger
    {
        public static string LogPath = Environment.CurrentDirectory;
        public static void Log(string message)
        {
            try
            {
                var sw = new StreamWriter($"{LogPath}\\PayPalLogs.log", true);
                sw.WriteLine($"{DateTime.Now} --------------- {message}");
            }
            catch (Exception)
            {

            }
        }
    }
}
