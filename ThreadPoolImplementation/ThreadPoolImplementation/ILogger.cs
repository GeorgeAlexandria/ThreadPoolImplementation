using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadPoolImplementation
{
  interface ILogger
  {
    void LogError(string message, Exception exception);

    void LogMessage(string message);
  }
}
