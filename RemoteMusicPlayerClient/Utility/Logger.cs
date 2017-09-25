using System.IO;

namespace RemoteMusicPlayerClient.Utility
{
    public class Logger
    {
        private static readonly string _logFile = "./log.txt";
        public static void Log(string value)
        {
            File.AppendAllText(_logFile, value + "\n");
        }
    }
}