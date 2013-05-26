using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XServer
{
    public class Logger
    {
        static string fileName = "";
        static StreamWriter sWriter;

        public static bool ConsoleOutput = false;

        public static void Init()
        {
            string file = "XServer Log "+DateTime.Now.ToString("MM.dd")+".xlog";

            fileName = Directory.GetCurrentDirectory() + "\\" + file;
            sWriter = new StreamWriter(new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite));

            if (sWriter.BaseStream.Length > 0)
            {
                sWriter.BaseStream.Position = sWriter.BaseStream.Length - 1;
            } 
        }

        public static void Log(string msg, HttpRequest request = null)
        {
            string date = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss GMT+3");

            string line = "[" + date + "]" + "[notice] [" + (request != null ? "client " + request.RemoteIp + "] " : "client unknown] ") + msg;


            WriteLine(line);
            //"[Wed Oct 11 14:32:52 2000] [error] [client 127.0.0.1] client denied by server configuration: /export/home/live/ap/htdocs/test";
        }

        public static void LogError(string msg, HttpRequest request = null)
        {
            string date = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss GMT+3");

            string line = "[" + date + "]" + "[error] [" + (request != null ? "client " + request.RemoteIp + "] " : "client unknown] ") + msg;


            WriteLine(line);
        }

        public static void LogWarning(string msg, HttpRequest request = null)
        {
            string date = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss GMT+3");

            string line = "[" + date + "]" + "[warning] [" + (request != null ? "client " + request.RemoteIp + "] " : "client unknown] ") + msg;


            WriteLine(line);
        }

        static void WriteLine(string line)
        {
            sWriter.WriteLine(line);
            sWriter.Flush();

            if (ConsoleOutput)
                Console.WriteLine(line);
        }

    }
}
