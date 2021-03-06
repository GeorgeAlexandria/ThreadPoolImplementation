﻿using System;
using System.Threading;

namespace ThreadPoolImplementation
{
  public class ConsoleLogger : ILogger
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

  public static class ExceptionHelper
  {
    public static void SafeHandler(Action unsafeAction, ILogger logger)
    {
      try
      {
        unsafeAction?.Invoke();
      }
      catch (Exception exception)
      {
        logger.LogError(exception.Message, exception);
        // Need throw or work next?
      }
    }
  }

  internal static class ThreadHelper
  {
    // Unfortunately number of workers need to pick up
    internal static readonly int ThreadsCount = Environment.ProcessorCount;

    internal static void SafeHandlerEnter(Action criticalAction, object lockObject)
    {
      try
      {
        Monitor.Enter(lockObject);
        criticalAction?.Invoke();
      }
      finally
      {
        Monitor.PulseAll(lockObject);
        Monitor.Exit(lockObject);
      }
    }

    internal static void SafeHandlerTryEnter(Action criticalAction, Action notCriticalAction, object lockObject)
    {
      bool isLocked = false;
      try
      {
        isLocked = Monitor.TryEnter(lockObject);
        if (isLocked)
        {
          criticalAction?.Invoke();
        }
        else
        {
          notCriticalAction?.Invoke();
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