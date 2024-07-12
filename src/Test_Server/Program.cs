using Atmip.Framework.ServicesWorker.Config;
using Newtonsoft.Json;
using ClosedXML.Excel;
using System.Globalization;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Drawing;

namespace TransactionalBankingSimulation
{
  class Program
  {
    public static string cycleName = "";
    // Global Path variable for the project
    public static string dataDirectory = "C:\\Users\\guillotn\\_work\\filesForAtmIp\\data";

    
    static void Main(string[] args)
    {
      try
      {
        ParseParams.parse(args);

        // Create the name/path of each cycle with the iteration, the writing method, and the size (in octets)
        cycleName = $"cycle_{ParseParams.iteration}_{ParseParams.writingMethod}_{ParseParams.size}";
        string cyclePath = System.IO.Path.Join(dataDirectory, cycleName);

        TransactionalReaderWritter.Init(System.IO.Path.Join(dataDirectory, cycleName), GlyAppLogger.GlyAppLogger.Current);

        // If the writeTx method is selected, we generate our transactions
        if (ParseParams.methodWriteTx)
        {
          // Generate the text to write in the transaction depending on the size given
          string generatedText = LoremIpsum.GenerateLoremIpsum(int.Parse(ParseParams.size));

          Directory.CreateDirectory(cyclePath);

          // Write transactions until a key is pressed
          while (!Console.KeyAvailable)
          {
            TransactionalReaderWritter.Begin();
            TransactionalReaderWritter.Write(generatedText, cycleName);
            TransactionalReaderWritter.Commit();

          }
        }
        // If the analyze method is selected, we analyze the cycle selected and store it into an Excel file 
        else if (ParseParams.methodAnalyze)
        {
          // Analyze the cycle given and export the results to an Excel file
          CycleAnalysisResult analysisResults = CycleAnalysisResult.AnalyzeResults(cyclePath);
          ExportExcel.exportExcel(analysisResults, ParseParams.writingMethod, ParseParams.size);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"exception{ex}");
      }
    }
  }
}

