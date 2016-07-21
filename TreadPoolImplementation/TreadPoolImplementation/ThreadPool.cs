using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreadPoolImplementation
{
  public sealed class ThreadPool
  {
    private static readonly ThreadPool instance = new ThreadPool();

    public static ThreadPool Instance
    {
      get
      {
        return instance;
      }
    }

    private ThreadPool()
    {
    }
  }
}
