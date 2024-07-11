using System.Collections.Generic;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Atmip.Framework.ServicesWorker.Config
{
  public static class TransactionalReaderWritter
  {
    private const byte MAX_TRANSACTIONS = 100;
    private const string TRANSACTION_FORMAT = "yyyy-MM-ddT_HH'h'mm'min'ss.fffffff'sec'z";

    private static string RootDataPath { get; set; } = string.Empty;
    private static GlyAppLogger.IAppLogger? TraceLogger;

    private static DateTimeOffset CurrentTransaction { get; set; }
    private static string CurrentTransactionString
    {
      get
      {
        return CurrentTransaction.ToString(TRANSACTION_FORMAT);
      }
    }

    private static IEnumerable<DateTimeOffset> KnownFailedTransactions
    {
      get
      {
        IEnumerable<DateTimeOffset> transactions = Enumerable.Empty<DateTimeOffset>();
        try
        {
          DirectoryInfo transactionsDirInfo = new(RootDataPath);
          transactions = transactionsDirInfo.GetDirectories()
                                    .Where(x => {
                                      return DateTimeOffset.TryParseExact(x.Name, TRANSACTION_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                                             && !CheckCommit(DateTimeOffset.ParseExact(x.Name, TRANSACTION_FORMAT, CultureInfo.InvariantCulture));
                                    })
                                    .Select(s => DateTimeOffset.ParseExact(s.Name, TRANSACTION_FORMAT, CultureInfo.InvariantCulture))
                                    .ToArray();
        }
        catch
        {
          TraceLogger?.Trace("TransactionalReaderWritter.KnownFailedTransactions", "attempted to read transactions but the class has not been initialized");
        }
        return transactions;
      }
    }
    private static IEnumerable<DateTimeOffset> KnownValidTransactions
    {
      get
      {
        IEnumerable<DateTimeOffset> transactions = Enumerable.Empty<DateTimeOffset>();
        try
        {
          DirectoryInfo transactionsDirInfo = new(RootDataPath);
          transactions = transactionsDirInfo.GetDirectories()
                                    .Where(x => {
                                      return DateTimeOffset.TryParseExact(x.Name, TRANSACTION_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                                             && CheckCommit(DateTimeOffset.ParseExact(x.Name, TRANSACTION_FORMAT, CultureInfo.InvariantCulture));
                                    })
                                    .Select(s => DateTimeOffset.ParseExact(s.Name, TRANSACTION_FORMAT, CultureInfo.InvariantCulture))
                                    .ToArray();
          //foreach (var transaction in transactions)
          //{
          //  TraceLogger?.Trace("TransactionalReaderWritter.KnownValidTransactions", $"valid transactions {transaction.ToString()}");
          //}
        }
        catch
        {
          TraceLogger?.Trace("TransactionalReaderWritter.KnownValidTransactions", "attempted to read transactions but the class has not been initialized");
        }
        return transactions;
      }
    }

    private static bool CheckCommit(DateTimeOffset transaction)
    {
      string dataPath = Path.Join(RootDataPath, transaction.ToString(TRANSACTION_FORMAT));
      string readData = SecureReaderWritter.Read<string>(dataPath, "commit.txt");
      return !string.IsNullOrWhiteSpace(readData);
    }

    //demander au readwriter la liste des transactions. on check en ordre descendant jusqu'à tomber sur un commit valide
    public static void Init(string rootDataPath_, GlyAppLogger.IAppLogger traceLogger_)
    {
      RootDataPath = rootDataPath_;
      TraceLogger = traceLogger_;
      TraceLogger?.Trace($"TransactionalReaderWritter.Init:", $"RootDataPath: {RootDataPath}");
    }

    //set CurrentTransaction to DateTimeOffset.Now
    public static void Begin()
    {
      CurrentTransaction = DateTimeOffset.Now;
      string transactionDir = Path.Join(RootDataPath, CurrentTransactionString);
      Directory.CreateDirectory(transactionDir);
      TraceLogger?.Trace("TransactionalReaderWritter.Begin", $"CurrentTransaction: {CurrentTransactionString}");
    }

    //write in CurrentTransaction dir
    public static bool Write<T>(T item, string instanceName = "", string option = "WriteThrough")
    {
      string dataPath = Path.Join(RootDataPath, CurrentTransactionString);
      bool result = SecureReaderWritter.Write<T>(item, dataPath, instanceName, TraceLogger, option);
      return result;
    }

    //read in latest ValidTransaction dir
    public static T? Read<T>(string instanceName = "")
    {
      T? result = default(T);
      if (KnownValidTransactions.Any())
      {
        string dataPath = Path.Join(RootDataPath, KnownValidTransactions.Max().ToString(TRANSACTION_FORMAT));
        result = SecureReaderWritter.Read<T>(dataPath, instanceName);
      }
      else
      {

      }
      return result;
    }

    //write a commit file in currenttransaction dir.
    //if there is more than MAX_transactionS, delete the older ones until only MAX_transactionS transactions remain
    public static void Commit()
    {
      string dataPath = Path.Join(RootDataPath, CurrentTransactionString);
      //SecureReaderWritter.CreateInitialSHA(dataPath);
      SecureReaderWritter.Write("commit", dataPath, "commit.txt", null, "WriteThrough");

      //delete failed transactions
      ClearFailedTransactions();

      //delete older transactions
      ClearOlderTransactions();
    }

    private static bool ClearOlderTransactions()
    {
      bool result = false;
      try
      {
        while (KnownValidTransactions.Count() > MAX_TRANSACTIONS)
        {
          string transactionPath = Path.Join(RootDataPath, KnownValidTransactions.Min().ToString(TRANSACTION_FORMAT));
          DirectoryInfo oldestTransactionInfo = new(transactionPath);
          foreach (FileInfo file in oldestTransactionInfo.GetFiles())
          {
            file.Delete();
          }
          oldestTransactionInfo.Delete();
          TraceLogger?.Trace("TransactionalReaderWritter.ClearOlderTransactions", $"deleted oldest transaction {oldestTransactionInfo.Name}");
        }
      }
      catch (Exception ex)
      {
        TraceLogger?.Trace("TransactionalReaderWritter.ClearOlderTransactions", ex);
      }
      return result;
    }

    private static bool ClearFailedTransactions()
    {
      bool result = false;
      foreach (var transaction in KnownFailedTransactions)
      {
        try
        {
          DirectoryInfo transactionsDirInfo = new(Path.Join(RootDataPath, transaction.ToString(TRANSACTION_FORMAT)));
          foreach (FileInfo file in transactionsDirInfo.GetFiles())
          {
            file.Delete();
          }
          transactionsDirInfo.Delete();
          TraceLogger?.Trace("TransactionalReaderWritter.ClearFailedTransactions", $"deleted failed transaction {transaction.ToString(TRANSACTION_FORMAT)}");
        }
        catch (Exception ex)
        {
          TraceLogger?.Trace("TransactionalReaderWritter.ClearFailedTransactions", ex);
        }
      }

      return result;
    }
  }
}
