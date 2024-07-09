using Atmip.Framework.ServicesWorker.Config;
using Newtonsoft.Json;
using ClosedXML.Excel;

namespace TransactionalBankingSimulation
{
    class Program
    {
        public static bool methodWriteTx = false;
        public static bool methodAnalyze = false;
        public static string paramPath = "";
        public static string size = "";
        public static string dataDirectory = "C:\\Users\\guillotn\\_work\\filesForAtmIp\\data";
        public static string filesForAtmIp = "C:\\Users\\guillotn\\_work\\filesForAtmIp";
        public static string writingMethod = "";

        static void parse(IEnumerable<string> args)
        {
            foreach (string arg in args) {
                if (arg == "-writeTx") {
                    methodWriteTx = true;
                } else if (arg == "-analyze") {
                    methodAnalyze = true;
                } else if (arg.StartsWith("-path=")) {
                    paramPath = arg.Substring(6);
                } else if (arg.StartsWith("-size=")) {
                    size = arg.Substring(6);
                } else if (arg.StartsWith("-method=")) {
                    writingMethod = arg.Substring(8);;
                }

            }

        }

        static void Main(string[] args)
        {
            try
            {
                parse(args); 

                if (methodWriteTx)
                {
                    int cycleNumber = 1;
                    paramPath = $"cycle_{cycleNumber}_{writingMethod}";
                    string targetDirectory = Path.Join(dataDirectory, paramPath);
                    while (Directory.Exists(targetDirectory))
                    {
                        cycleNumber++;
                        paramPath = $"cycle_{cycleNumber}_{writingMethod}";
                        targetDirectory = Path.Join(dataDirectory, paramPath);
                    }
                    Directory.CreateDirectory(targetDirectory);

                    TransactionalReaderWritter.Init(Path.Join(dataDirectory, paramPath), GlyAppLogger.GlyAppLogger.Current);
                    int totalNumberOfFolders = 20;
                    int transactionsPerFile = int.Parse(size);

                    DateTime startTime = DateTime.Now;

                    for (int currentFileIndex = 1; currentFileIndex <= totalNumberOfFolders; currentFileIndex++)
                    {
                        List<Transaction> transactions = new();
                        string fileName = $"transactions_{currentFileIndex}.json";
                        TransactionalReaderWritter.Begin();
                        for (int i = 1; i <= transactionsPerFile; i++)
                        {
                            try
                            {
                                Transaction transaction = GenerateTransaction(i);
                                transactions.Add(transaction);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Erreur lors de la transaction {i} du fichier {currentFileIndex}: {ex.Message}");
                            }
                        }
                        TransactionalReaderWritter.Write(transactions, fileName);
                        TransactionalReaderWritter.Commit();

                    }
                    DateTime endTime = DateTime.Now;

                    TimeSpan elapsedTime = endTime - startTime;

                    string csvFilePath = Path.Join(dataDirectory, $"write_times_{writingMethod}.csv");
                    if (!File.Exists(csvFilePath))
                    {
                        using (StreamWriter sw = new StreamWriter(csvFilePath, false))
                        {
                            sw.WriteLine(elapsedTime.TotalSeconds);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(csvFilePath, true))
                        {
                            sw.WriteLine(elapsedTime.TotalSeconds);
                        }
                    }

                    Console.WriteLine("Transactions terminées");
                }
                else if (methodAnalyze)
                {
                    string analyzeDirectory = "C:\\Users\\guillotn\\_work\\filesForAtmIp\\analyzes";
                    TransactionalReaderWritter.Init(Path.Join(dataDirectory, paramPath), GlyAppLogger.GlyAppLogger.Current);
                    AnalyzeResults analysisResults = AnalyzeResults(Path.Join(dataDirectory, paramPath));
                    CreateAnalyseFile(Path.Join(dataDirectory, paramPath), analysisResults);

                    // Conversion JSON -> Excel
                    string[] jsonFiles = Directory.GetFiles(analyzeDirectory, "*.json");
                    if (jsonFiles.Length > 0)
                    {
                        // vérifier si le fichier excel existe, si c'est le cas, on s'en servira pour ajouter les données, sinon on le créera
                        XLWorkbook workbook;
                        string excelFilePath = Path.Join(filesForAtmIp, "analyzed_data.xlsx");
                        if (File.Exists(excelFilePath))
                        {
                            workbook = new XLWorkbook(excelFilePath);
                        }
                        else
                        {
                            workbook = new XLWorkbook();
                        }
                        var worksheet = workbook.Worksheets.Add($"Analyse_{writingMethod}_HDD");
                        int currentRow = 1;

                        // Écriture des en-têtes
                        worksheet.Cell(currentRow, 1).Value = "Numero de cycle";
                        worksheet.Cell(currentRow, 2).Value = "Nombre total de dossiers";
                        worksheet.Cell(currentRow, 3).Value = "Nombre de dossiers corrompus";
                        worksheet.Cell(currentRow, 4).Value = "Taille attendue";
                        worksheet.Cell(currentRow, 5).Value = "Taille reelle";
                        worksheet.Cell(currentRow, 6).Value = "Temps d'écriture (s)";
                        worksheet.Cell(currentRow, 7).Value = "Moyenne de temps d'écriture (s)";
                        currentRow++;

                        // Écriture des données de l'analyse
                        string csvFilePath = Path.Join(dataDirectory, $"write_times_{writingMethod}.csv");
                        var csvLines = File.ReadAllLines(csvFilePath);
                        for (int i = 0; i < csvLines.Length; i++)
                        {
                            worksheet.Cell(currentRow, 1).Value = i + 1;
                            worksheet.Cell(currentRow, 2).Value = analysisResults.totalFolders;
                            worksheet.Cell(currentRow, 3).Value = analysisResults.corruptedFolders;
                            worksheet.Cell(currentRow, 4).Value = $"{((float.Parse(size)*170)*20) * 10e-7} Mo";
                            worksheet.Cell(currentRow, 5).Value = $"{CalculateTotalSizeInMo(Path.Join(dataDirectory, $"cycle_{i+1}_{writingMethod}"))} Mo";
                            worksheet.Cell(currentRow, 6).Value = double.Parse(csvLines[i]);
                            currentRow++;
                        }
                        worksheet.Cell(2, 9).FormulaA1 = $"AVERAGE(H2:H{currentRow - 1})";

                        // Sauvegarde du fichier Excel
                        workbook.SaveAs(excelFilePath);
                        Console.WriteLine($"Données converties en Excel : {excelFilePath}");
                    }
                    else
                    {
                        Console.WriteLine("Aucun fichier JSON trouvé dans le répertoire spécifié.");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'exécution du programme {ex}");
            }
        }

        // Générateur de transactions fictives
        static Transaction GenerateTransaction(int id)
        {
            Random random = new();
            return new Transaction
            {
                TransactionId = id,
                Date = DateTime.Now,
                Sender = $"Client_{random.Next(1, 100)}",
                Receiver = $"Client_{random.Next(101, 200)}",
                Amount = random.Next()
            };
        }

        static AnalyzeResults AnalyzeResults(string dataDirectory)
        {
            int totalFolders = Directory.GetDirectories(dataDirectory).Length;
            int corruptedFolders = 0;
            List<string> corruptedFoldersNames = new();

            foreach (string directoryPath in Directory.GetDirectories(dataDirectory))
            {
                string commitFilePath = Path.Join(directoryPath, "commit.txt");

                if (!File.Exists(commitFilePath)) // Vérification de la présence de commit.txt
                {
                    corruptedFolders++;
                    corruptedFoldersNames.Add(Path.GetFileName(directoryPath));
                }
            }

            AnalyzeResults analysisResults = new()
            {
                totalFolders = totalFolders,
                corruptedFolders = corruptedFolders,
                corruptedFoldersNames = corruptedFoldersNames
            };

            return analysisResults;
        }

        static void CreateAnalyseFile(string AnalyzeDirectory, AnalyzeResults analysisResults)
        {
            string analyzeDirectory = "C:\\Users\\guillotn\\_work\\filesForAtmIp\\analyzes";
            int analyzeNumber = 1;

            while (File.Exists(Path.Join(analyzeDirectory, $"analyze_{analyzeNumber}.json")))
            {
                analyzeNumber++;
            }
            File.WriteAllText(Path.Join(analyzeDirectory, $"analyze_{analyzeNumber}.json"),
            JsonConvert.SerializeObject(analysisResults, Formatting.Indented));
            Console.WriteLine("Analyse effectuee");

        }

        static double CalculateTotalSizeInMo(string directoryPath)
        {
            double totalSizeInBytes = 0;
            
            // On passe sur tous les dossiers du répertoire
            foreach (var folder in Directory.EnumerateDirectories(directoryPath))
            {
                // On recupere la taille de chaque fichier
                foreach (var file in Directory.EnumerateFiles(folder))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    totalSizeInBytes += fileInfo.Length;
                }
            }

            return Math.Round(totalSizeInBytes * 10e-7, 2);
        }
    }
    class Transaction
    {
        public int TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public double Amount { get; set; }
    }

    public class AnalyzeResults
    {
        public int totalFolders { get; set; }
        public int corruptedFolders { get; set; }
        public List<string> corruptedFoldersNames { get; set; }

    }


}
