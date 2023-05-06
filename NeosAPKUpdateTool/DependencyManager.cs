using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Net.Http;
using NeosAPKPatchingTool.Config;

namespace NeosAPKPatchingTool
{
    public class DependencyManager
    {
        public static string MainDirectory { get; set; } = string.Empty;
        public static string DepDirectory { get; set; } = string.Empty;
        private List<APKDependency> _dependencies;
        public DependencyManager()
        {
            MainDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DepDirectory = Directory.CreateDirectory(Path.Combine(MainDirectory, "Dependencies")).FullName;

            _dependencies = new List<APKDependency>()
            {
                new APKDependency()
                {
                    Name = "APKTool",
                    Version = "v2.7.0",
                    DownloadURL = "https://github.com/iBotPeaches/Apktool/releases/download/v2.7.0/apktool_2.7.0.jar"
                },
                new APKDependency()
                {
                    Name = "uber-apk-signer",
                    Version = "v1.3.0",
                    DownloadURL = "https://github.com/patrickfav/uber-apk-signer/releases/download/v1.3.0/uber-apk-signer-1.3.0.jar"
                },
                new APKDependency()
                {
                    Name = "Android ADB",
                    Version = "latest",
                    FolderName = "platform-tools",
                    DownloadURL = "https://dl.google.com/android/repository/platform-tools-latest-windows.zip"
                }
            };
        }

        public void CheckInstalled()
        {
            CheckHasJava();
            List<APKDependency> missingDeps = new List<APKDependency>();

            foreach (var dep in _dependencies)
            {
                if (dep.FolderName != null) {
                    string dirname = Path.Combine(DepDirectory, dep.FolderName);
                    if (Directory.Exists(dirname)) continue;
                }

                if (!File.Exists(Path.Combine(DepDirectory, dep.FileName))) {
                    missingDeps.Add(dep);
                }
            }

            if (missingDeps.Count == 0) return;

            Console.WriteLine("You seem to be missing the following dependencies:");
            foreach (var dep in missingDeps)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0} (@{1})\nfrom {2}\n", dep.Name, dep.Version, dep.DownloadURL);
                Console.ForegroundColor = ConsoleColor.Gray;

            }

            bool shouldDL = ConfigManager.Config.AutoDownloadDeps;
            if (!shouldDL)
            {
                Console.WriteLine("Would you like to download these dependencies? (y/n)");
                shouldDL = Program.PromptUser();
                Console.WriteLine();
            }

            if (shouldDL) DownloadDependencies(missingDeps.ToArray()).Wait();
            else {
                Console.WriteLine("Exiting...");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
        }

        public void CheckHasJava()
        {
            if (!PatchingHandler.ExecuteJava("", "-version"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Java not installed!");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("This tool requires Java 8+ to extract, build and sign the NeosVR APK.\nPlease install Java to continue.");
                Thread.Sleep(10000);
                Environment.Exit(1);
            }
        }

        public void AddModLoaderDeps()
        {
            List<APKDependency> modLoaderDeps = new List<APKDependency>()
            {
                new APKDependency()
                {
                    Name = "NeosModLoader",
                    Version = "latest",
                    DownloadURL = "https://github.com/neos-modding-group/NeosModLoader/releases/latest/download/NeosModLoader.dll"
                },
                new APKDependency()
                {
                    Name = "0Harmony",
                    Version = "latest",
                    DownloadURL = "https://github.com/neos-modding-group/NeosModLoader/releases/latest/download/0Harmony.dll"
                }
            };
            _dependencies.AddRange(modLoaderDeps.Except(_dependencies));
        }

        public async Task DownloadDependencies(APKDependency[] dependencies)
        {
            var client = new HttpClient();

            Task[] downloads = new Task[dependencies.Length];
            for (int i = 0; i < dependencies.Length; i++) {
                downloads[i] = DownloadDependency(dependencies[i], client);
            }
            await Task.WhenAll(downloads);
            Console.WriteLine("Finished downloading {0} dependencies.\n", dependencies.Length);
        }
        public async Task DownloadDependency(APKDependency dependency, HttpClient client)
        {
            try
            {
                Console.WriteLine("Downloading {0}...", dependency.Name);
                Stream stream = await client.GetStreamAsync(dependency.DownloadURL);

                if (Path.GetExtension(dependency.DownloadURL) == ".zip")
                {
                    new ZipArchive(stream).ExtractToDirectory(DepDirectory);
                }
                else
                {
                    FileStream fileStream = new FileStream(Path.Combine(DepDirectory, dependency.FileName), FileMode.Create);
                    await stream.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }

                await stream.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching dependency '{0}':\n{1}", dependency.Name, ex.Message);
                Thread.Sleep(5000);
                Environment.Exit(1);
            }
        }

    }


    public struct APKDependency
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string DownloadURL { get; set; }
        public string FileName { get => Path.GetFileName(DownloadURL); }
        public string? FolderName { get; set; }
    }
}
