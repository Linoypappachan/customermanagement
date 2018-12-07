namespace SVC_CustomerManagement_Utilities.LoggerUtil
{
    public static class Logger
    {
       private static NLog.ILogger log { get; set; }

        static Logger()
        {
            log = NLog.LogManager.GetLogger(typeof(Logger).ToString());
        }

        public static void Error(object msg)
        {
            log.Error(msg);           
        }

        public static void Info(object msg)
        {
            log.Info(msg);
        }

        public static void Debug(object msg)
        {
            log.Debug(msg);
        }
    }
}