using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionalBankingSimulation
{
  public class ParseParams
  {
    public static bool methodWriteTx = false;
    public static bool methodAnalyze = true;
    public static string size = "1000";
    public static string writingMethod = "WriteThrough";
    public static string iteration = "first";
    public static int maxTx = 200;

    // Gather the arguments passed to the program
    public static void parse(IEnumerable<string> args)
    {
      foreach (string arg in args)
      {
        if (arg == "-writeTx")
        {
          methodWriteTx = true;
          methodAnalyze = false;
        }
        else if (arg == "-analyze")
        {
          methodAnalyze = true;
          methodWriteTx = false;
        }
        else if (arg.StartsWith("-size="))
        {
          size = arg.Substring(6);
        }
        else if (arg.StartsWith("-method="))
        {
          writingMethod = arg.Substring(8);
        }
        else if (arg.StartsWith("-iteration="))
        {
          iteration = arg.Substring(11);
        }
        else if (arg.StartsWith("-maxTx="))
        {
          maxTx = int.Parse(arg.Substring(7));
        }

      }
    }
  }
}
