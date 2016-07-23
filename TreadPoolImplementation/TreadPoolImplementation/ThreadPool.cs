using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TreadPoolImplementation
{
  public sealed class ThreadPool
  {
    private readonly LinkedList<Action> tasks = new LinkedList<Action>();
    private readonly List<Worker> workers;

    private object tasksLock = new object();
    private object criticalLock = new object();

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

    public void QueueUserWorkTask(Action task)
    {
      lock (tasksLock)
      {
        tasks.AddLast(task);
      }
    }

  }
}
