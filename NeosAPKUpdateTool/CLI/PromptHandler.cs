using NeosAPKPatchingTool.Config;
using System.IO;

namespace NeosAPKPatchingTool.CLI
{
    internal class PromptHandler
    {

        public static bool PromptUser()
        {
            char key = 'a';
            while (key != 'y' && key != 'n')
            {
                key = Console.ReadKey().KeyChar;
                Console.CursorLeft = Math.Max(Console.CursorLeft - 1, 0);
            }
            return key == 'y';
        }

        // TODO: Remove once the latest release of NML has Android compatibility.
        public static void DisplayNMLNotice()
        {
            if (!ConfigManager.Config.InjectModLoader || File.Exists(Path.Combine(DependencyManager.DepDirectory, "NeosModLoader.dll"))) return;
            Console.WriteLine("\nPlease self-compile NeosModLoader.dll and place it in your Dependencies folder.\nAlternatively, you can download the latest artifact from GitHub Actions.");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("https://github.com/neos-modding-group/NeosModLoader/suites/13045347898/artifacts/707111506");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\nOnce you have everything set up, relaunch this program.");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static void DisplayHelp()
        {
            Console.WriteLine("Usage: NeosAPKPatchingTool \"APKPath\" \"DataPath\" [args]\n");
            Console.WriteLine("-h: Display this help message.");
            Console.WriteLine("-f: Automatically download any missing dependencies.");
            Console.WriteLine("-m: Patch the input APK with NeosModLoader.");
            Console.WriteLine("-d: Patch APK with 'debuggable' attribute.\n");
            Console.WriteLine("--fingers: Patch APK with native finger tracking support.");
        }


#if WINDOWS
        public static string OpenAPKSelection()
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

        public static string OpenFolderSelection()
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Please select the Neos_Data folder from your PC installation.";
            dialog.UseDescriptionForTitle = true;
            Console.WriteLine(dialog.Description);

            string path = "";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path_managed = Path.Combine(dialog.SelectedPath, "Managed");
                if (Directory.Exists(path_managed))
                {
                    path = dialog.SelectedPath;
                }
            }
            return path;
        }
#else
        public static string OpenAPKSelection()
        {
            Console.WriteLine("Please enter the path to your NeosOculus.apk file:");
            string? path = Console.ReadLine();
            if (string.IsNullOrEmpty(path) || Path.GetExtension(path) != ".apk") return "";
            return path;
        }

        public static string OpenFolderSelection()
        {
            Console.WriteLine("Please enter the path of your PC installation's Neos_Data folder:");
            string? path = Console.ReadLine();

            if (path == null) return "";

            string path_managed = Path.Combine(path, "Managed");
            return Directory.Exists(path_managed) ? path : "";
        }
#endif
    }
}
