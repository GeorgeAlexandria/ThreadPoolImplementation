using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TestProject
{
  public struct Tuple<T1, T2>
  {
    public T1 first;

    public T2 second;

    public Tuple(T1 first, T2 second)
    {
      this.first = first;
      this.second = second;
    }
  }

  public static class Representation
  {
    private enum ErrorType
    {
      Success,
      InvalidNumberArguments,
      InvalidFormat,
      InvalidArgument,
      FileNotExists,
    }

    private static readonly int minNumberBytes = 1;
    private static readonly int maxNumberBytes = 0x400 * 0x400 * 100;

    public static void WriteInformationAboutProject()
    {
      Console.WriteLine("This is test project, which divide input file on blocks, compute their hash and write they to console.");
    }

    public static Tuple<string, int> ReadData()
    {
      string text;
      string additionalErrorInfo = String.Empty;
      Tuple<string, int> data = new Tuple<string, int>();
      while (true)
      {
        Console.WriteLine($"{additionalErrorInfo}Please enter the path to file and size of each blocks in byte.");
        Console.WriteLine();
        text = Console.ReadLine();
        switch (ParseInputData(text, ref data))
        {
          case ErrorType.Success:
            return data;
          case ErrorType.InvalidNumberArguments:
            additionalErrorInfo = "\nError: Invalid count of input argument.\n";
            break;
          case ErrorType.InvalidFormat:
            additionalErrorInfo = "\nError: Cann't parse input data.\n";
            break;
          case ErrorType.InvalidArgument:
            additionalErrorInfo = $"\nError: Invalid input number of byte. Please enter number of bytes between {minNumberBytes} and {maxNumberBytes} bytes.\n";
            break;
          case ErrorType.FileNotExists:
            additionalErrorInfo = $"\nError: Input file doesn't exist.\n";
            break;
        }
      }
    }

    private static ErrorType ParseInputData(string text, ref Tuple<string, int> data)
    {
      // divide by space
      string[] args = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

      if (args.Length != 2)
      {
        return ErrorType.InvalidNumberArguments;
      }
      if (!File.Exists(args[0]))
      {
        return ErrorType.FileNotExists;

      }

      int parseInt = 0;
      if (!int.TryParse(args[1], out parseInt))
      {
        return ErrorType.InvalidFormat;
      }
      if (parseInt < minNumberBytes || parseInt > maxNumberBytes)
      {
        return ErrorType.InvalidArgument;
      }

      data.first = args[0];
      data.second = parseInt;
      return ErrorType.Success;
    }
  }
}
