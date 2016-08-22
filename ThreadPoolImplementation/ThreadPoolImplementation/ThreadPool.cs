using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadPoolImplementation
{
  public sealed class ThreadPool : IDisposable
  {
    private class Worker
    {
      public Thread thread;

      private ILogger logger;

      public Worker(ILogger logger)
      {
        this.logger = logger;
      }

      public void Start()
      {
        // Safe on all work action, it may be useful in future.
        thread = new Thread(() => { ExceptionHelper.SafeHandler(Work, logger); });
        thread.Start();
      }

      private void Work()
      {
        while (!Instance.isDisposed)
        {
          ExceptionHelper.SafeHandler(() =>
          {
            Action task = null;
            ThreadHelper.SafeHandlerTryEnter(() =>
            {
              if (Instance.tasks.First != null)
              {
                task = Instance.tasks.First.Value;
                Instance.tasks.RemoveFirst();
              }
            }, null, Instance.tasksLock);
            task?.Invoke();
          }, logger);
        }
      }
    }

    private readonly LinkedList<Action> tasks = new LinkedList<Action>();
    private readonly List<Worker> workers;
    private readonly int countWorkers = ThreadHelper.ThreadsCount;

    private object tasksLock = new object();

    private bool isDisposed = false;

    private static readonly ThreadPool instance = new ThreadPool();

    public static ThreadPool Instance
    {
      get
      {
        return instance;
      }
    }

    static ThreadPool()
    {
    }

    private ThreadPool()
    {
      workers = new List<Worker>(countWorkers);
      for (int i = 0; i < countWorkers; i++)
      {
        Worker worker = new Worker(new ConsoleLogger());
        worker.Start();
        workers.Add(worker);
      }
    }

    public void QueueUserWorkTask(Action task)
    {
      lock (tasksLock)
      {
        tasks.AddLast(task);
        Monitor.PulseAll(tasksLock);
      }
    }

    public void Dispose()
    {
      bool waitWorkers = false;
      lock (tasksLock)
      {
        if (!isDisposed)
        {
          while (tasks.Count > 0)
          {
            Monitor.Wait(tasksLock);
          }

          isDisposed = true;
          Monitor.PulseAll(tasksLock);
          waitWorkers = true;
        }
      }
      if (waitWorkers)
      {
        workers.ForEach(worker => worker.thread.Join());
      }
    }
  }
}
