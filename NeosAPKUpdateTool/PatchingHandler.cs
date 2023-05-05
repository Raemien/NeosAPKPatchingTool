using NeosAPKPatchingTool.Config;
using NeosAPKPatchingTool.Modding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NeosAPKPatchingTool
{
    public class PatchingHandler
    {
        public static string WorkingPath { get { return Path.Combine(DependencyManager.DepDirectory, "apk"); } }
        internal string OutputPath {
            get {
                var info = new DirectoryInfo(APKPath);
                return info.Parent.FullName;
            }
        }

        internal string APKPath;
        internal string DataPath;
        public PatchingHandler(string apkpath, string datapath)
        {
            APKPath = apkpath;
            DataPath = datapath;
        }
        public static bool ExecuteJava(string jar = "", string args = "")
        {
            bool nojar = (jar == "");
            string jarpath = Path.Combine(DependencyManager.DepDirectory, jar).Replace('\\', '/');
            if (!nojar) args = string.Format("-jar \"{0}\" ", jarpath) + args;

            try
            {
                ProcessStartInfo javainfo = new ProcessStartInfo();
                javainfo.FileName = "java.exe";
                javainfo.WorkingDirectory = nojar ? "" : WorkingPath;
                javainfo.Arguments = args;
                javainfo.UseShellExecute = false;
                javainfo.RedirectStandardOutput = true;
                javainfo.RedirectStandardError = nojar;

                Process? javaproc = Process.Start(javainfo);
                if (javaproc == null) throw new Exception("Unable to launch java!");
                javaproc.WaitForExit();
                return true;
            }
            catch (Exception ex)
            {
                if (!nojar) {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(5000);
                    Environment.Exit(1);
                }
                return false;
            }

        }
        public void PatchAPK()
        {
            Directory.CreateDirectory(WorkingPath);

            Console.WriteLine("Unpacking APK...");
            ExecuteJava("apktool_2.7.0.jar", string.Format(" d \"{0}\"", APKPath));

            Console.WriteLine("Replacing old binaries...");
            string apk_name = Path.GetFileNameWithoutExtension(APKPath);
            string extract_path = Path.Combine(WorkingPath, apk_name);
            string bin_path = Path.Combine(DataPath, "Managed");
            string bin_path_apk = Path.Combine(extract_path, "assets/bin/Data/Managed");

            foreach (var bin in Directory.GetFiles(bin_path))
            {
                if (!bin.EndsWith("Assembly-CSharp.dll"))
                {
                    string bin_name = Path.GetFileName(bin);
                    string newpath = Path.Combine(bin_path_apk, bin_name);
                    File.Copy(bin, newpath, true);
                }
            }

            if (ConfigManager.Config.InjectModLoader)
            {
                Console.WriteLine("Injecting NeosModLoader...");
                string nmlPath = Path.Combine(DependencyManager.DepDirectory, "NeosModLoader.dll");
                File.Copy(nmlPath, bin_path_apk + "/NeosModLoader.dll", true);
                var injector = new InjectionHandler(bin_path_apk);
                injector.PatchFroox();
            }

            Console.WriteLine("Patching AndroidManifest...");
            var patcher = new ManifestPatcher(extract_path);
            patcher.PatchManifest();

            Console.WriteLine("Repacking APK...");
            ExecuteJava("apktool_2.7.0.jar", string.Format(" b \"{0}\" -o \"{0}.apk\"", extract_path));

            Console.WriteLine("Resigning APK...");
            string apk_params = string.Format("--apks \"{0}.apk\"", extract_path);
            ExecuteJava("uber-apk-signer-1.3.0.jar", apk_params);

            string newapk = Path.Combine(OutputPath, apk_name + "-patched.apk");
            Console.WriteLine("Copying APK to {0}...", newapk);
            File.Copy(extract_path + "-aligned-debugSigned.apk", newapk, true);

            Console.WriteLine("Cleaning up...");
            Directory.Delete(WorkingPath, true);

            Console.WriteLine("\nDone!");
            Thread.Sleep(3000);
            Environment.Exit(0);
        }
    }
}
