using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Atmip.Framework.ServicesWorker.Config
{
  public static class SecureReaderWritter
  {
    public static void WriteSHA(string fileName, byte[] hashValue, GlyAppLogger.IAppLogger traceLogger = null, string option = "WriteThrough")
    {
      try
      {
        using (FileStream fileStream = new($"{fileName}.sha", FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 0x1000, StringToFileOptions(option)))
        {
          fileStream.Write(hashValue, 0, hashValue.Length);
          fileStream.Flush();
          fileStream.Close();
        }
      }
      catch (IOException e)
      {
        traceLogger?.Trace("SecureReaderWritter.WriteSHA: I/O Exception", e);
      }
      catch (UnauthorizedAccessException e)
      {
        traceLogger?.Trace("SecureReaderWritter.WriteSHA: Access Exception", e);
      }
    }

    public static byte[] Read(string fileName)
    {
      byte[] content = null;

      try
      {
        content = File.ReadAllBytes(fileName);
        //using (FileStream fileStream = File.Open(fileName, FileMode.Open))
        //{

        //  // Create a fileStream for the file.
        //  // Be sure it's positioned to the beginning of the stream.
        //  content = new byte[fileStream.Length];
        //  int numBytesToRead = (int)fileStream.Length;
        //  int numBytesRead = 0;


        //  fileStream.Read(content, numBytesRead, numBytesToRead);
        //  fileStream.Close();
        //  // Compute the hash of the fileStream.*
        //}
      }
      catch (IOException e)
      {
        Console.WriteLine($"I/O Exception: {e.Message}");
      }
      catch (UnauthorizedAccessException e)
      {
        Console.WriteLine($"Access Exception: {e.Message}");
      }
      return content;
    }

    public static byte[] ComputeSHA(MemoryStream sourceStream, string key = "", GlyAppLogger.IAppLogger traceLogger = null)
    {
      MemoryStream memoryStream = new();
      sourceStream.WriteTo(memoryStream);

      byte[] hashValue = Array.Empty<byte>();
      using (SHA256 mySHA256 = SHA256.Create())
      {
        try
        {
          UTF8Encoding encoding = new();
          memoryStream.Write(encoding.GetBytes(key), 0, key.Length);
          memoryStream.Seek(0, SeekOrigin.Begin);

          hashValue = mySHA256.ComputeHash(memoryStream);
        }
        catch (Exception e)
        {
          traceLogger?.Trace("SecureReaderWritter.ComputeSHA", e);
        }
      }
      return hashValue;
    }

    public static byte[] ComputeSHA(string fileName, string key = "", GlyAppLogger.IAppLogger traceLogger = null)
    {
      byte[] hashValue = Array.Empty<byte>();
      using (SHA256 mySHA256 = SHA256.Create())
      {
        try
        {
          using (FileStream fileStream = File.Open(fileName, FileMode.Open))
          {
            // Create a fileStream for the file.
            // Be sure it's positioned to the beginning of the stream.
            fileStream.Position = 0;
            // Compute the hash of the fileStream.
            MemoryStream memoryStream = new();
            fileStream.CopyTo(memoryStream);

            hashValue = ComputeSHA(memoryStream, key, traceLogger);

            fileStream.Flush();
            fileStream.Close();
          }
        }
        catch (IOException e)
        {
          traceLogger?.Trace("SecureReaderWritter.ComputeSHA: I/O Exception", e);
        }
        catch (UnauthorizedAccessException e)
        {
          traceLogger?.Trace("SecureReaderWritter.ComputeSHA: Access Exception", e);
        }
      }
      return hashValue;
    }

    public static bool CheckSHA(byte[] shaToCheck, MemoryStream memoryStream,string key = "")
    {
      byte[] hashValue = ComputeSHA(memoryStream, key);
      return hashValue.SequenceEqual(shaToCheck);
    }

    public static bool CheckSHA(string fileName, string key = "", GlyAppLogger.IAppLogger traceLogger = null)
    {
      bool result = false;
      byte[] hashValue = ComputeSHA(fileName, key);
      try
      {
        using (FileStream fileStream = File.Open($"{fileName}.sha", FileMode.Open))
        {
          // Create a fileStream for the file.
          // Be sure it's positioned to the beginning of the stream.
          fileStream.Position = 0;

          byte[] content = new byte[fileStream.Length];
          int numBytesToRead = (int)fileStream.Length;
          int numBytesRead = 0;
          fileStream.Read(content, numBytesRead, numBytesToRead);
          fileStream.Close();

          // Compute the hash of the fileStream.*
          if (hashValue.SequenceEqual(content))
          {
            result = true;
          }
        }
      }
      catch (IOException e)
      {
        traceLogger?.Trace("SecureReaderWritter.CheckSHA: I/O Exception", e);
      }
      catch (UnauthorizedAccessException e)
      {
        traceLogger?.Trace("SecureReaderWritter.CheckSHA: Access Exception", e);
      }
      return result;
    }


    public static T ReadWithSHA<T>(string configPath, GlyAppLogger.IAppLogger traceLogger = null)
    {
      T result = default;
      try
      {
        if (!string.IsNullOrEmpty(configPath))
        {
          if (File.Exists(configPath))
          {
            if (CheckSHA(configPath))
            {
              result = JsonConvert.DeserializeObject<T>(File.ReadAllText(configPath));
            }
          }
        }
      }
      catch (Exception ex)
      {
        traceLogger?.Trace($"SecureReaderWritter.ReadWithSHA({configPath})", ex);
      }
      return result;
    }

    private static void CreateInitialSHA(string fileName)
    {
      if (File.Exists(fileName) && !File.Exists($"{fileName}.sha"))
      {
        WriteSHA(fileName, ComputeSHA(fileName));
      }
    }

    private static FileOptions StringToFileOptions(string option)
    {
        switch (option)
        {
            case "WriteThrough":
                return FileOptions.WriteThrough;
            case "Asynchronous":
                return FileOptions.Asynchronous;
            case "None":
                return FileOptions.None;
            // Ajoutez d'autres cas au besoin
            default:
                return FileOptions.None;
        }
    }


        //public static void WriteWithShaTwoTimes<T>(T actorConfig, string path, GlyAppLogger.IAppLogger traceLogger = null)
        //{
        //  WriteWithSha<T>(actorConfig, path + ".reboot", traceLogger);
        //  WriteWithSha<T>(actorConfig, path, traceLogger);
        //}

    private static bool WriteWithSHA<T>(T actorConfig, string path, GlyAppLogger.IAppLogger traceLogger = null, string option = "WriteThrough")
    {
        bool result = true;

      try
      {
        using (FileStream fs = new(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 0x1000, StringToFileOptions(option)))
        {
            // using (StreamWriter file = File.CreateText(Path))
            using (StreamWriter streamWriter = new(fs))
            {
                JsonSerializer serializer = new()
                {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize(streamWriter, actorConfig, typeof(T));
                streamWriter.Flush();
                fs.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                WriteSHA(path, ComputeSHA(path));
            }
        }
      }
      catch (Exception ex)
      {
        traceLogger?.Trace("SecureReaderWritter.WriteWithSHA", ex);
        result = false;
      }
      return result;
    }

    #region public interface

    public static T Read<T>(string rootDataPath, string configFileName, GlyAppLogger.IAppLogger traceLogger = null)
    {
      T result = default;
      string filePath = Path.Join(rootDataPath, configFileName);
      try
      {
        if (File.Exists(filePath))
        {
          CreateInitialSHA(filePath);
          result = ReadWithSHA<T>(filePath, traceLogger);
          if (result == null)
          {
            throw new Exception("Invalid SHA");
          }
        }
        else
        {
          throw new Exception("file not exist");
        }


        //Get the default root path
        return result;
      }
      catch (Exception ex)
      {
        traceLogger?.Trace($"SecureReaderWritter.Read({filePath})", ex);
      }
      return default;
    }

    public static bool Write<T>(T actorConfig, string rootDataPath, string configFileName, GlyAppLogger.IAppLogger traceLogger = null, string option = "WriteThrough")
    {
      bool result = true;
      string filePath = Path.Join(rootDataPath, configFileName);
      try
      {
        WriteWithSHA<T>(actorConfig, filePath, traceLogger,  option);
      }
      catch (Exception ex)
      {
        traceLogger?.Trace($"SecureReaderWritter.Write({filePath})", ex);
        result = false;
      }
      return result;
    }

    #endregion
  }
}
