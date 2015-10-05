using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vAutomtion.Logger.LoggerManager;

namespace KapitalTradingEngine
{
    public class Program
    {
        static void Main(string[] args)
        {
            SetUpLogger();

            LoggerManager.Log(Level.Debug, "Starting KapitalTradingEngine");
            try
            {
                KapitalTradingEngine kapitalTradingEngine = new KapitalTradingEngine();

                kapitalTradingEngine.Initialize();

                while (kapitalTradingEngine.IsRunning)
                {
                    kapitalTradingEngine.Run();
                }
            }
            catch (Exception ex)
            {
                LoggerManager.Log(Level.Error, "KapitalTradingEngine encoutnered a fatal error: "+ex.Message);
            }

            Console.WriteLine("KapitalTradingEngine terminated. Please press any key.");
            Console.ReadKey();
        }

        private static void SetUpLogger()
        {
            if (ConfigurationManager.AppSettings["LogFilePath"] == null)
            {
                throw new ArgumentNullException("LogFilePath", "Missing LogFilePath in confguration file.");
            }

            string logFilePath = ConfigurationManager.AppSettings["LogFilePath"].ToString();
            LoggerManager.SetLogger(logFilePath);
        }
    }
}
