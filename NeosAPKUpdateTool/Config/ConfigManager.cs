namespace NeosAPKPatchingTool.Config
{
    internal class ConfigManager
    {
        public static Configuration Config { get; set; } = Configuration.Default;

        private List<string> _args;
        public ConfigManager(ref string[] args)
        {
            _args = args.ToList();
            Config = GetConfigFromArguments();
            args = _args.ToArray();
        }

        public Configuration GetConfigFromArguments()
        {
            Configuration config = new Configuration();

            config.InjectModLoader = ParseBool("-m");
            config.AutoDownloadDeps = ParseBool("-f");
            config.UseHandTracking = ParseBool("--fingers");
            config.Debuggable = ParseBool("-d");
            return config;

        }

        public bool ParseBool(string pattern)
        {
            return _args.Remove(pattern);
        }
    }

    internal class Configuration
    {
        public string? APKPath { get; set; }
        public string? DataPath { get; set; }
        public bool InjectModLoader { get; set; }
        public bool AutoDownloadDeps { get; set; }
        public bool Debuggable { get; set; }
        public bool UseHandTracking { get; set; }
        public static Configuration Default => new Configuration();
    }

}
