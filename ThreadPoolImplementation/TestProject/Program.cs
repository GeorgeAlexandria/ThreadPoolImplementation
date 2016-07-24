using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ThreadPoolImplementation;

namespace TestProject
{
  class Program
  {
    static Action<long, int, byte[]> taskHashBytes = delegate (long blockNumber, int countBytes, byte[] buffer)
    {
      if (buffer.Length < countBytes)
      {
        throw new ArgumentException("Stream length less than input count of bytes.");
      }
      byte[] hash = SHA256.Create().ComputeHash(buffer, 0, countBytes);
      Console.WriteLine($"Number block: {blockNumber}\nCompute hash: {BitConverter.ToString(hash).Replace("-", String.Empty)}");
    };

    static Action<long, int, string> taskReadBytesAndHash = delegate (long blockNumber, int countBytes, string pathToFile)
    {
      byte[] buffer = new byte[countBytes];
      long offset = blockNumber * countBytes;
      int readedCountBytes = 0;
      if (countBytes > 2 * megabyte)
      {
        int subTaskCount = Environment.ProcessorCount;
        int countExecutedSubTasks = 0;
        int countToRead = countBytes / subTaskCount;
        for (int i = 0; i < subTaskCount; ++i)
        {
          int taskNumber = i;
          new Thread(() =>
          {
            using (BufferedStream reader = new BufferedStream(File.OpenRead(pathToFile), megabyte))
            {
              long newOffset = offset + taskNumber * countToRead;
              if (reader.Seek(newOffset, SeekOrigin.Begin) != newOffset)
              {
                throw new EndOfStreamException($"Cannot seek a stream to position {newOffset}");
              }
              int readedCount = reader.Read(buffer, taskNumber * countToRead, countToRead);
              Interlocked.Increment(ref countExecutedSubTasks);
              Interlocked.Add(ref readedCountBytes, readedCount);
            }
          }).Start();
        }
        while (countExecutedSubTasks != subTaskCount) ;
      }
      else
      {
        using (BufferedStream reader = new BufferedStream(File.OpenRead(pathToFile), megabyte))
        {
          if (reader.Seek(offset, SeekOrigin.Begin) != offset)
          {
            throw new EndOfStreamException($"Cannot seek a stream to position {offset}");
          }
          readedCountBytes = reader.Read(buffer, 0, countBytes);
        }
      }
      taskHashBytes(blockNumber, countBytes, buffer);
    };

    private static int megabyte = 1024 * 1024;

    static void Main(string[] args)
    {
      Representation.WriteInformationAboutProject();
      Tuple<string, int> data = Representation.ReadData();

      string pathToFile = data.first;
      int countBytes = data.second;
      long repeat = new FileInfo(pathToFile).Length / countBytes + 1;
      using (ThreadPoolImplementation.ThreadPool.Instance)
      {
        for (long i = 0; i < repeat; ++i)
        {
          long blockNumber = i;
          ThreadPoolImplementation.ThreadPool.Instance.QueueUserWorkTask(() =>
          {
            taskReadBytesAndHash(blockNumber, countBytes, pathToFile);
          });
        }
      }
    }
  }
}
