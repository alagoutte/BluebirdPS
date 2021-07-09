using BluebirdPS.Models;
using System.IO;

namespace BluebirdPS.Core
{
    internal class Config
    {
        internal static readonly string configurationPath = Path.Join(Helpers.GetFileSavePath(), "Configuration.json");
        internal static readonly string credentialsPath = Path.Join(Helpers.GetFileSavePath(), "twittercred.sav");

        private static Configuration configuration;

        private static Configuration Create()
        {
            // create .BluebirdPS folder
            if (!Directory.Exists(Helpers.GetFileSavePath()))
            {
                _ = Directory.CreateDirectory(Helpers.GetFileSavePath());
            }

            // create config file
            if (!File.Exists(configurationPath))
            {
                _ = File.Create(configurationPath);
                configuration = new Configuration()
                {
                    AuthUserId = "292670084",
                    AuthUserName = "thedavecarroll",
                    RateLimitAction = RateLimitAction.Warning,
                    RateLimitThreshold = 5,
                    ConfigurationPath = configurationPath,
                    CredentialsPath = credentialsPath,
                    OutputType = OutputType.BluebirdPS
                };

                ExportConfiguration();
            }
            else
            {
                ImportConfiguration();
            }

            return configuration;
        }

        public static Configuration GetOrCreateInstance() => configuration ??= Create();

        internal static void ImportConfiguration()
        {
            configuration = JsonConverters.ConvertFromJson<Configuration>(File.ReadAllText(configurationPath));
        }

        internal static void ExportConfiguration()
        {
            string config = JsonConverters.ConvertToJson(configuration);
            File.WriteAllTextAsync(configurationPath, config);
        }

    }
}
