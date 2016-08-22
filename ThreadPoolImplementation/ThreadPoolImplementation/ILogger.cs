using System;

namespace ThreadPoolImplementation
{
  public interface ILogger
  {
    void LogError(string message, Exception exception);

    void LogMessage(string message);
  }
}