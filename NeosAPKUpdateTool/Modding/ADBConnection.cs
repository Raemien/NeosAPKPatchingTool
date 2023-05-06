using System.Diagnostics;
using System.IO;

namespace NeosAPKPatchingTool.Modding
{
    internal class ADBConnection
    {
        public static string ADBPath { get { return Path.Combine(DependencyManager.DepDirectory, "platform-tools"); } }
        public static bool DeviceConnected()
        {
            return ExecuteADB("devices -l").Contains("device:");
        }

        public static string ExecuteADB(string command = "")
        {
            try
            {
                ProcessStartInfo adbinfo = new ProcessStartInfo();
                adbinfo.FileName = "adb.exe";
                adbinfo.WorkingDirectory = ADBPath;
                adbinfo.Arguments = command;
                adbinfo.UseShellExecute = false;
                adbinfo.RedirectStandardOutput = true;
                adbinfo.RedirectStandardError = true;

                Process? adbproc = Process.Start(adbinfo);
                if (adbproc == null) throw new Exception("Unable to launch ADB!");
                return adbproc.StandardOutput.ReadToEnd().Replace(Environment.NewLine, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ADB Error: {0}", ex.Message);
                Thread.Sleep(5000);
                return "";
            }
        }
    }
}
