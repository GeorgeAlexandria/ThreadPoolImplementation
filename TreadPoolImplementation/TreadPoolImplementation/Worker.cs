using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TreadPoolImplementation
{
  public class Worker
  {
    private Thread thread;
    // This list always must contains a one empty node with null action
    public readonly LinkedList<Action> localTasks = new LinkedList<Action>();

    public Action FirstTask
    {
      get
      {
        Action result = null;
        if (Monitor.TryEnter(localTasks.First))
        {
          try
          {
            result = localTasks.First.Value;
            if (result != null)
            {
              localTasks.RemoveFirst();
            }
          }
          finally
          {
            Monitor.Exit(localTasks.First);
          }
        }
        return result;
      }
    }

    private Action LastTask
    {
      get
      {
        Action result = null;
        if (Monitor.TryEnter(localTasks.Last))
        {
          try
          {
            result = localTasks.Last.Value;
            if (result != null)
            {
              localTasks.RemoveLast();
            }
          }
          finally
          {
            Monitor.Exit(localTasks.Last);
          }
        }
        return result;
      }
    }

    public Worker(Action task, string name)
    {
      localTasks.AddFirst(new LinkedListNode<Action>(null));
      localTasks.AddLast(task);
    }

    public void Start()
    {
      thread = new Thread(Work);
      thread.Start();
    }

    private void Work()
    {

    }
  }
}
