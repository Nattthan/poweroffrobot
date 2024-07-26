using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System;
using System.Runtime.InteropServices;
using System.Text;

public class Kernel32
{
  // Import the CreateFile function from kernel32.dll
  [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  static extern IntPtr CreateFile(
      string lpFileName,
      uint dwDesiredAccess,
      uint dwShareMode,
      IntPtr lpSecurityAttributes,
      uint dwCreationDisposition,
      uint dwFlagsAndAttributes,
      IntPtr hTemplateFile);

  // Import the WriteFile function from kernel32.dll
  [DllImport("kernel32.dll", SetLastError = true)]
  static extern bool WriteFile(
      IntPtr hFile,
      byte[] lpBuffer,
      uint nNumberOfBytesToWrite,
      out uint lpNumberOfBytesWritten,
      IntPtr lpOverlapped);

  // Import the CloseHandle function from kernel32.dll
  [DllImport("kernel32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  static extern bool CloseHandle(IntPtr hObject);

  // Constants for dwDesiredAccess
  const uint GENERIC_WRITE = 0x40000000;

  // Constants for dwShareMode
  const uint FILE_SHARE_NONE = 0x0;

  // Constants for dwCreationDisposition
  const uint CREATE_ALWAYS = 2;

  // Constants for dwFlagsAndAttributes
  const uint FILE_ATTRIBUTE_NORMAL = 0x80;
  const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;

  static public void writeFile(string fileName, byte[] buffer)
  {

    // Create the file with FILE_FLAG_WRITE_THROUGH
    IntPtr hFile = CreateFile(fileName, GENERIC_WRITE, FILE_SHARE_NONE, IntPtr.Zero, CREATE_ALWAYS, FILE_FLAG_WRITE_THROUGH, IntPtr.Zero);

    if (hFile == IntPtr.Zero || hFile == (IntPtr)(-1))
    {
      Console.WriteLine("Failed to create file. Error: " + Marshal.GetLastWin32Error());
      return;
    }

    // Write to the file
    bool result = WriteFile(hFile, buffer, (uint)buffer.Length, out uint bytesWritten, IntPtr.Zero);

    if (!result)
    {
      Console.WriteLine("Failed to write to file. Error: " + Marshal.GetLastWin32Error());
      CloseHandle(hFile);
      return;
    }

    Console.WriteLine($"Successfully wrote {bytesWritten} bytes to {fileName}");

    // Close the file
    if (!CloseHandle(hFile))
    {
      Console.WriteLine("Failed to close file handle. Error: " + Marshal.GetLastWin32Error());
    }
  }
}
