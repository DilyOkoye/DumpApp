using System;
using System.IO;

namespace DumpApp.BAL.Utilities
{
    public static class LogManager
    {

        public static string EncodeWithString(string inputFileName)
        {
            byte[] binaryData;

            using (FileStream inFile = new System.IO.FileStream(inputFileName,
                                                  System.IO.FileMode.Open,
                                                  System.IO.FileAccess.Read))
            {
                binaryData = new Byte[inFile.Length];
                long bytesRead = inFile.Read(binaryData, 0,
                                            (int)inFile.Length);
                string base64String = string.Empty;
                try
                {
                    base64String =
                       System.Convert.ToBase64String(binaryData,
                                                     0,
                                                     binaryData.Length);
                }
                catch (System.ArgumentNullException)
                {
                    System.Console.WriteLine("Binary data array is null.");

                }
                return base64String;
            }

            return null;

        }

        private static Object cvLockObject = new Object();
        private static string cvsLogFile = System.Configuration.ConfigurationManager.AppSettings["LogFile"];
        private static string filePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"];
        private static string LogSize = System.Configuration.ConfigurationManager.AppSettings["LogSize"];

        public static double MonthDifference(this DateTime lValue, DateTime rValue)
        {
            return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);
        }
        public static void SaveLog(string psDetails)
        {
            if (cvsLogFile != null)
            {
                FileInfo f = new FileInfo(cvsLogFile);

                if (File.Exists(cvsLogFile))
                {
                    long s1 = f.Length;
                    if (s1 > Convert.ToInt32(LogSize))
                    {
                        string filename = Path.GetFileNameWithoutExtension(cvsLogFile) + string.Format("{0:yyyyMMddhhttss}", DateTime.Now) + "RevWeb" + ".txt";
                        if (File.Exists(Path.Combine(filePath, filename)))
                            File.Delete(Path.Combine(filePath, filename));
                        File.Move(cvsLogFile, Path.Combine(filePath, filename));
                        f.Delete();
                    }
                }
            }
            lock (cvLockObject)
            {
                if (cvsLogFile != null)
                {

                    File.AppendAllText(Path.Combine(cvsLogFile), DateTime.Now.ToString() + ": " + psDetails + Environment.NewLine);
                }
            }
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

    }
}
