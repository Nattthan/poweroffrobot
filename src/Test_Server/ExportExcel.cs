using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionalBankingSimulation
{
  public class ExportExcel
  {
    public static string filesForAtmIp = "C:\\Users\\guillotn\\_work\\filesForAtmIp";

    // Export the results to an Excel file (one line for each cycle)
    public static void exportExcel(CycleAnalysisResult analysisResult, string writingMethod, string size)
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
  }
}
