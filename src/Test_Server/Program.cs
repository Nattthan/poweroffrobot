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
    public static bool methodWriteTx = false;
    public static bool methodAnalyze = false;
    public static string cycleName = "";
    public static string iteration = "";
    public static string size = "";
    public static string writingMethod = "";

    // Pat
    public static string dataDirectory = "C:\\Users\\guillotn\\_work\\filesForAtmIp\\data";
    public static string filesForAtmIp = "C:\\Users\\guillotn\\_work\\filesForAtmIp";

    static void parse(IEnumerable<string> args)
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
        else if (arg.StartsWith("-path="))
        {
          cycleName = arg.Substring(6);
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

    static void exportExcel(CycleAnalysisResult analysisResult)
    {
      XLWorkbook workbook;
      string excelFilePath = System.IO.Path.Join(filesForAtmIp, "analyzed_data.xlsx");
      int currentRow = 1;
      if (File.Exists(excelFilePath))
      {
        workbook = new XLWorkbook(excelFilePath);
      }
      else
      {
        workbook = new XLWorkbook();
      }
      IXLWorksheet worksheet;
      string sheetName = $"Analyse_{writingMethod}";

      if (!workbook.Worksheets.TryGetWorksheet(sheetName, out worksheet))
      {
        worksheet = workbook.Worksheets.Add(sheetName);
        currentRow = 1;
        // Écriture des en-têtes
        worksheet.Cell(currentRow, 1).Value = "Numero de cycle";
        worksheet.Cell(currentRow, 2).Value = "Nombre total de dossiers";
        worksheet.Cell(currentRow, 3).Value = "Nombre de dossiers corrompus";
        worksheet.Cell(currentRow, 4).Value = "Taille";
        worksheet.Cell(currentRow, 5).Value = "Temps d'écriture min (ms)";
        worksheet.Cell(currentRow, 6).Value = "Temps d'écriture max (ms)";
        worksheet.Cell(currentRow, 7).Value = "Temps d'écriture moy (ms)";
      }
      currentRow = worksheet.LastRowUsed().RowNumber() + 1;

      // Écriture des données
      worksheet.Cell(currentRow, 1).Value = analysisResult.CycleName;
      worksheet.Cell(currentRow, 2).Value = analysisResult.TotalFolders;
      worksheet.Cell(currentRow, 3).Value = analysisResult.CorruptedFolders;
      worksheet.Cell(currentRow, 4).Value = int.Parse(size);

      // Calculate the differences in milliseconds
      List<double> differences = new List<double>();
      for (int i = 1; i < analysisResult.CreationTimes.Count; i++)
      {
        TimeSpan timeSpan = analysisResult.CreationTimes[i] - analysisResult.CreationTimes[i - 1];
        differences.Add(timeSpan.TotalMilliseconds);
      }

      // Calculate min, max, and average
      double minDifference = differences.Min();
      double maxDifference = differences.Max();
      double avgDifference = differences.Average();

      worksheet.Cell(currentRow, 5).Value = minDifference;
      worksheet.Cell(currentRow, 6).Value = maxDifference;
      worksheet.Cell(currentRow, 7).Value = avgDifference;


      // Output the results
      Console.WriteLine($"Minimum difference in milliseconds: {minDifference}");
      Console.WriteLine($"Maximum difference in milliseconds: {maxDifference}");
      Console.WriteLine($"Average difference in milliseconds: {avgDifference}");
      currentRow++;

      // Sauvegarde du fichier Excel
      workbook.SaveAs(excelFilePath);
      Console.WriteLine($"Données converties en Excel : {excelFilePath}");
    }
    static void Main(string[] args)
    {
      try
      {
        parse(args);

        cycleName = $"cycle_{iteration}_{writingMethod}_{size}";
        string cyclePath = System.IO.Path.Join(dataDirectory, cycleName);

        TransactionalReaderWritter.Init(System.IO.Path.Join(dataDirectory, cycleName), GlyAppLogger.GlyAppLogger.Current);

        if (methodWriteTx)
        {

          string generatedText = LoremIpsum.GenerateLoremIpsum(int.Parse(size));

          Directory.CreateDirectory(cyclePath);

          while (!Console.KeyAvailable)
          {
            TransactionalReaderWritter.Begin();
            TransactionalReaderWritter.Write(generatedText, cycleName);
            TransactionalReaderWritter.Commit();

          }
        }
        else if (methodAnalyze)
        {
          CycleAnalysisResult analysisResults = AnalyzeResults(cyclePath);
          exportExcel(analysisResults);
          // vérifier si le fichier excel existe, si c'est le cas, on s'en servira pour ajouter les données, sinon on le créera
        }
      }
      catch(Exception ex)
      {
         Console.WriteLine($"exception{ex}");

      }

      static CycleAnalysisResult AnalyzeResults(string cyclePath)
        {
        var cycleResult = new CycleAnalysisResult
                  {
            CycleName = System.IO.Path.GetFileName(cyclePath),
            TotalFolders = Directory.GetDirectories(cyclePath).Length,
            CreationTimes = new List<DateTimeOffset>()
        };

          foreach (string txPath in Directory.GetDirectories(cyclePath))
          {
            string format = "yyyy-MM-ddT_HH'h'mm'min'ss.fffffff'sec'z";
            string folderName = txPath.Split('\\').Last();
          // Parse the string to DateTimeOffset
          var CreationTime = DateTimeOffset.ParseExact(folderName, format, CultureInfo.InvariantCulture);
          cycleResult.CreationTimes.Add(CreationTime);

          string commitFilePath = System.IO.Path.Join(txPath, "commit.txt");
            if (File.Exists(commitFilePath)) // Vérification de la présence de commit.txt
            {
              string commitTxt = SecureReaderWritter.ReadWithSHA<string>(commitFilePath);

              if (commitTxt == null)
              {
                cycleResult.CorruptedFolders++;
              }

              // cycleResult.CorruptedFoldersNames.Add(Path.GetFileName(txPath));
            }
            else
            {
              cycleResult.CorruptedFolders++;
            }
          }

          return cycleResult;
      }

        // static double CalculateTotalSizeInMo(string cyclePath)
        // {
        //   double totalSizeInBytes = 0;

        //   // On passe sur tous les dossiers du répertoire
        //   foreach (var folder in Directory.EnumerateDirectories(cyclePath))
        //   {
        //     // On recupere la taille de chaque fichier
        //     foreach (var file in Directory.EnumerateFiles(folder))
        //     {
        //       FileInfo fileInfo = new FileInfo(file);
        //       totalSizeInBytes += fileInfo.Length;
        //     }
        //   }

        //   return Math.Round(totalSizeInBytes * 10e-7, 2);
        // }
      
      }
    }

    class CycleAnalysisResult
    {
      public string CycleName { get; set; }
    public int TotalFolders { get; set; }
    public int CorruptedFolders { get; set; }
      // public List<string> CorruptedFoldersNames { get; set; } = new List<string>();
      public List<DateTimeOffset> CreationTimes { get; set; }
    }

  }

