using Atmip.Framework.ServicesWorker.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionalBankingSimulation
{
  public class CycleAnalysisResult
  {
    public string CycleName { get; set; }
    public int TotalFolders { get; set; }
    public int CorruptedFolders { get; set; }
    // public List<string> CorruptedFoldersNames { get; set; } = new List<string>();
    public List<DateTimeOffset> CreationTimes { get; set; }

    // Take a given Cycle to analyze it and return the results (number of folders, corrupted folders, creation times, etc.)
    public static CycleAnalysisResult AnalyzeResults(string cyclePath)
    {
      var cycleResult = new CycleAnalysisResult
      {
        CycleName = System.IO.Path.GetFileName(cyclePath),
        TotalFolders = Directory.GetDirectories(cyclePath).Length,
        CreationTimes = new List<DateTimeOffset>()
      };

      // We go through all the folders in the cycle directory
      foreach (string txPath in Directory.GetDirectories(cyclePath))
      {
        // We get the creation time of each folder (we parse the folder name to get the creation time as a DateTimeOffset object instead of a stringS)
        string format = "yyyy-MM-ddT_HH'h'mm'min'ss.fffffff'sec'z";
        string folderName = txPath.Split('\\').Last();
        var CreationTime = DateTimeOffset.ParseExact(folderName, format, CultureInfo.InvariantCulture);
        cycleResult.CreationTimes.Add(CreationTime);

        // We check if the commit.txt file is present in the folder
        string commitFilePath = System.IO.Path.Join(txPath, "commit.txt");
        if (File.Exists(commitFilePath))
        {
          // if yes, we read the content of the file and if the .sha is also here then we check if the content is corrupted
          string commitTxt = SecureReaderWritter.ReadWithSHA<string>(commitFilePath);

          if (commitTxt == null)
          {
            cycleResult.CorruptedFolders++;
          }
        }
        else
        {
          cycleResult.CorruptedFolders++;
        }
      }

      // We return the result
      return cycleResult;
    }
  }
}
