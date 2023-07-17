﻿using NeosAPKPatchingTool.CLI;
using NeosAPKPatchingTool.Config;

namespace NeosAPKPatchingTool
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ConfigManager configManager = new ConfigManager(ref args);
            if (configManager.ParseBool("-h")) {
                PromptHandler.DisplayHelp();
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
                config.InjectModLoader = PromptHandler.PromptUser();
                Console.WriteLine("\n");
            }

            // TODO: Remove once the latest release of NML has Android compatibility.
            PromptHandler.DisplayNMLNotice();

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
