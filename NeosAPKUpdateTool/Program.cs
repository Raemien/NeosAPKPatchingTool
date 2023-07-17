using NeosAPKPatchingTool.CLI;
using NeosAPKPatchingTool.Config;

namespace NeosAPKPatchingTool
{
    internal class Program
    {
        public static bool PromptUser()
        {
            char key = 'a';
            while (key != 'y' && key != 'n') {
                key = Console.ReadKey().KeyChar;
                Console.CursorLeft = Math.Max(Console.CursorLeft - 1, 0);
            }
            return key == 'y';
        }

        // TODO: Remove once the latest release of NML has Android compatibility.
        static void DisplayNMLNotice()
        {
            if (!ConfigManager.Config.InjectModLoader || File.Exists(Path.Combine(DependencyManager.DepDirectory, "NeosModLoader.dll"))) return;
            Console.WriteLine("\nPlease self-compile NeosModLoader.dll and place it in your Dependencies folder.\nAlternatively, you can download the latest artifact from GitHub Actions.");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("https://github.com/neos-modding-group/NeosModLoader/suites/12295795460/artifacts/651545830");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nOnce you have everything set up, relaunch this program.");
            Console.ReadKey();
            Environment.Exit(0);
        }

        static void DisplayHelp()
        {
            Console.WriteLine("Usage: NeosAPKPatchingTool \"APKPath\" \"DataPath\" [args]\n");
            Console.WriteLine("-h: Display this help message.");
            Console.WriteLine("-f: Automatically download any missing dependencies.");
            Console.WriteLine("-m: Patch the input APK with NeosModLoader.");
            Console.WriteLine("-d: Patch APK with 'debuggable' attribute.\n");
            Console.WriteLine("--fingers: Patch APK with native finger tracking support.");
        }
        [STAThread]
        static void Main(string[] args)
        {
            ConfigManager configManager = new ConfigManager(ref args);
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

            // TODO: Remove once the latest release of NML has Android compatibility.
            DisplayNMLNotice();

            if (config.InjectModLoader) depchecker.AddModLoaderDeps();
            depchecker.CheckInstalled();

            string apkpath = (args.Length > 0) ? args[0] : PromptHandler.OpenAPKSelection();
            if (apkpath == "") {
                Console.WriteLine("Invalid APK path.");
                Thread.Sleep(2000);
                Environment.Exit(1);
            }

            string datapath = (args.Length > 1) ? args[1] : PromptHandler.OpenFolderSelection();
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
