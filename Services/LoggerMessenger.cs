using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Globalization;
using NLog;

namespace ush4.Services.NLogger
{
    public enum enEventType
    {
        Info,
        Warning,
        Error,
    }

    public class LoggerMessenger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Object _lock = new Object();
        public static event Action<enEventType, String> ErrorOrWarningOccurredEvent;
        public static event Action<enEventType, String> ShowErrorOrWarningEvent;
        /*
        public LoggerMessenger()
        { 
        
        }*/



        public static void Info(String message, params Object[] vals)
        {
            lock (_lock)
            {
                logger.Info(String.Format(message, vals));
                
            }
            ErrorOrWarningOccurredEvent?.Invoke(enEventType.Info, message);
            
        }

        public static void Trace(String message)
        {
            lock (_lock)
            {
                logger.Trace(message);
            }
        }

        public static void Trace(byte[] message)
        {
            string hex = BitConverter.ToString(message);
            lock (_lock)
            {
                logger.Trace(hex);
            }
        }

        public static void Error(String message)
        {
            lock (_lock)
            {
                logger.Error(message);
            }
            ErrorOrWarningOccurredEvent?.Invoke(enEventType.Error, message);
        }

        public static void Error( Exception ex, String message)
        {
            //lock (_lock)
            //{
            //    logger.Error(message);
            //}
            ErrorOrWarningOccurredEvent?.Invoke(enEventType.Error, message);
            Exception(ex, message);
        }

        public static void Warning(String message)
        {
            lock (_lock)
            {
                logger.Warn(message);
            }
            ErrorOrWarningOccurredEvent?.Invoke(enEventType.Warning, message);
        }

        public static void Exception(Exception e)
        {
            lock (_lock)
            {
                ExceptionImpl(e);
            }
        }

        public static void Exception(Exception e, String msg, params Object[] vals)
        {
            lock (_lock)
            {
                logger.Error(String.Format(msg, vals));
                ExceptionImpl(e);
            }
        }


        public static void ShowInfo(String message, params Object[] vals)
        {
            lock (_lock)
            {
                logger.Info(String.Format(message, vals));

            }
            ErrorOrWarningOccurredEvent?.Invoke(enEventType.Info, message);
            ShowErrorOrWarningEvent?.Invoke(enEventType.Info, message);
        }


        public static void ShowWarning(String message)
        {
            lock (_lock)
            {
                logger.Warn(message);
            }
            ErrorOrWarningOccurredEvent?.Invoke(enEventType.Warning, message);
            ShowErrorOrWarningEvent?.Invoke(enEventType.Warning, message);
        }


        public static void ShowError(String message)
        {
            lock (_lock)
            {
                logger.Error(message);
            }
            ErrorOrWarningOccurredEvent?.Invoke(enEventType.Error, message);
            ShowErrorOrWarningEvent?.Invoke(enEventType.Error, message);
        }

        

        public static void ShowError(Exception ex)
        {
            lock (_lock)
            {
                // logger.Error(exmessage);
                ExceptionImpl(ex);
            }
            ErrorOrWarningOccurredEvent?.Invoke(enEventType.Error, ex.Message);
            ShowErrorOrWarningEvent?.Invoke(enEventType.Error, ex.Message);
        }

        public static void ShowError(Exception ex, String msg)
        {
            /*
            lock (_lock)
            {
                // logger.Error(exmessage);
                ExceptionImpl(ex);
            }*/
            Exception(ex, msg);
            ErrorOrWarningOccurredEvent?.Invoke(enEventType.Error, msg);
            ShowErrorOrWarningEvent?.Invoke(enEventType.Error, msg);
        }

        

        private static void ExceptionImpl(Exception e)
        {
            Exception next = e;
            while (next != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(next.Message.Split(new char[] { '\n' }).First());
                sb.Append(" (");
                sb.Append(e.GetType().ToString());
                sb.AppendLine(")");
                sb.AppendLine(next.StackTrace);
                logger.Error(sb.ToString());
                next = next.InnerException;
            }
        }

        

        
    }
}
