using NeosAPKPatchingTool.Config;
using System.IO;

namespace NeosAPKPatchingTool
{
    internal class Program
    {
        static string OpenAPKSelection()
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = "apk";
            dialog.Title = string.Format("Please select your NeosOculus.apk file.");
            dialog.FileName = "NeosOculus.apk";
            Console.WriteLine(dialog.Title);

            string path = "";
            if (dialog.ShowDialog() == DialogResult.OK && Path.GetExtension(dialog.FileName) == ".apk") {
                path = dialog.FileName;
            }
            return path;
        }

        static string OpenFolderSelection()
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Please select the Neos_Data folder from your PC installation.";
            dialog.UseDescriptionForTitle = true;
            Console.WriteLine(dialog.Description);

            string path = "";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path_managed = Path.Combine(dialog.SelectedPath, "Managed");
                if (Directory.Exists(path_managed)) {
                    path = dialog.SelectedPath;
                }
            }
            return path;
        }

        public static bool PromptUser()
        {
            char key = 'a';
            while (key != 'y' && key != 'n') {
                key = Console.ReadKey().KeyChar;
                Console.CursorLeft = Math.Max(Console.CursorLeft - 1, 0);
            }
            return key == 'y';
        }

        static void DisplayHelp()
        {
            Console.WriteLine("Usage: NeosAPKPatchingTool.exe \"APKPath\" \"DataPath\" [args]\n");
            Console.WriteLine("-h: Display this help message.");
            Console.WriteLine("-f: Automatically download any missing dependencies.");
            Console.WriteLine("-m: Patch the input APK with NeosModLoader.");
            Console.WriteLine("-d: Patch APK with 'debuggable' attribute.\n");
            Console.WriteLine("--fingers: Patch APK with native finger tracking support.");
        }
        [STAThread]
        static void Main(string[] args)
        {
            ConfigManager configManager = new ConfigManager(args);
            if (configManager.ParseBool("-h")) {
                DisplayHelp();
                Environment.Exit(0);
            }

            Console.WriteLine("-------------------------------------------------------------------------------------");
            Console.WriteLine("Welcome! This tool aims to restore functionality to Android releases of NeosVR.\nThe program fixes networking issues that were patched in the PC release of NEOS.\n");
            Console.WriteLine("NOTE: Android releases of Neos are not currently maintained by Solirax.\nThis tool is not officially supported by Solirax and may introduce stability issues.");
            Console.WriteLine("-------------------------------------------------------------------------------------\n");

            var depchecker = new DependencyManager();
            var config = ConfigManager.Config;

            if (!config.InjectModLoader) {
                Console.WriteLine("Would you like to patch your APK with NeosModLoader? (y/n)");
                config.InjectModLoader = PromptUser();
                Console.WriteLine("\n");
            }

            if (config.InjectModLoader) depchecker.AddModLoaderDeps();
            depchecker.CheckInstalled();

            string apkpath = (args.Length > 0) ? args[0] : OpenAPKSelection();
            if (apkpath == "") {
                Console.WriteLine("Invalid APK path.");
                Thread.Sleep(2000);
                Environment.Exit(1);
            }

            string datapath = (args.Length > 1) ? args[1] : OpenFolderSelection();
            if (datapath == "") {
                Console.WriteLine("Invalid Neos_Data path.");
                Thread.Sleep(2000);
                Environment.Exit(1);
            }

            var patcher = new PatchingHandler(apkpath, datapath);
            patcher.PatchAPK();
        }
    }
}
