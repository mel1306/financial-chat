using System.IO;
using Microsoft.Extensions.Configuration;

namespace FinancialChat.DB
{
    public sealed class AppSettings
    {
        private static AppSettings _instance = null;
        private static readonly object padlock = new object();
        public readonly string _connectionString = string.Empty;

        private AppSettings()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Development.json"), optional: false)
                .Build();

            _connectionString = configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
        }

        public static AppSettings Instance
        {
            get
            {
                lock (padlock)
                {
                    return _instance ??= new AppSettings();
                }
            }
        }

        public string ConnectionString => _connectionString;
    }
}