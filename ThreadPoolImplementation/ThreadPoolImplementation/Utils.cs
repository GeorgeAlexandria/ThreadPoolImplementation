using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadPoolImplementation
{
  internal class ConsoleLogger : ILogger
  {
    public void LogError(string message, Exception exception)
    {
      Console.WriteLine(message);
      Console.WriteLine(exception.StackTrace);
      Exception innerException = exception;
      while (null != (innerException = innerException.InnerException))
      {
        Console.WriteLine(message);
        Console.WriteLine(exception.StackTrace);
      }

      // Exception.ToString() write all info about exception and it inner exception
      //Console.WriteLine(exception.ToString());
    }

    public void LogMessage(string message)
    {
      Console.WriteLine(message);
    }
  }

  internal static class Utils
  {
    internal static void SafeHandleLog(Action action, ILogger logger)
    {
      try
      {
        action();
      }
      catch (Exception exception)
      {
        logger.LogError(exception.Message, exception);
        // Need throw or work next?
      }
    }

    internal static void SafeHandleFinalize(Action criticalAction, object lockObject)
    {
      bool isLocked = false;
      try
      {
        isLocked = Monitor.TryEnter(lockObject);
        if (isLocked)
        {
          criticalAction();
        }
      }
      finally
      {
        if (isLocked)
        {
          Monitor.PulseAll(lockObject);
          Monitor.Exit(lockObject);
        }
      }
    }
  }
}
