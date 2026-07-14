using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NeuLis.DataBase
{
    public class Log
    {
        public static void WriteLog(string msg)
        {
            string logFileName = DateTime.Now.ToString("yyyyMMddHH") + ".txt";


            //此处根据不同的项目类型用不同的方法取路径
            //string logPath = base.Context.Server.MapPath("") + @"\LOG";
            //string logPath = HttpContext.Current.Server.MapPath("") + @"\LOG";
            string logPath = AppDomain.CurrentDomain.BaseDirectory + @"\sqlLog";
            string fullPath = logPath + @"\" + logFileName;

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            using (StreamWriter writer = File.AppendText(fullPath))
            {
                ALog(msg, writer);
                writer.Close();
            }
        }
        private static void ALog(string logMessage, TextWriter writer)
        {
            writer.Write("\r\n 时间 : ");
            writer.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            writer.WriteLine("  :{0}", logMessage);
            writer.WriteLine("-------------------------------");
            writer.Flush();
        }
    }
}
