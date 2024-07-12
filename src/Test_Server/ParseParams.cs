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
    public static bool methodAnalyze = false;
    public static string size = "";
    public static string writingMethod = "";
    public static string iteration = "";

    // Gather the arguments passed to the program
    public static void parse(IEnumerable<string> args)
    {
      foreach (string arg in args)
      {
        if (arg == "-writeTx")
        {
          methodWriteTx = true;
        }
        else if (arg == "-analyze")
        {
          methodAnalyze = true;
        }
        else if (arg.StartsWith("-size="))
        {
          size = arg.Substring(6);
        }
        else if (arg.StartsWith("-method="))
        {
          writingMethod = arg.Substring(8); ;
        }
        else if (arg.StartsWith("-iteration="))
        {
          iteration = arg.Substring(11);
        }

      }
    }
  }
}
