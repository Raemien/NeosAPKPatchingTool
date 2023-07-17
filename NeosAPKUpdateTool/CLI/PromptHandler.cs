using System.IO;

namespace NeosAPKPatchingTool.CLI
{
    internal class PromptHandler
    {
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
