using FinancialChat.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialChat.DB
{
    public class OptionsBuilder
    {
        private static OptionsBuilder _instance = null;
        private static readonly object padlock = new object();
        public readonly DbContextOptionsBuilder<ApplicationDbContext> _options;

        private OptionsBuilder()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>();
            _options.UseSqlServer(AppSettings.Instance.ConnectionString);
        }

        public static OptionsBuilder Instance
        {
            get
            {
                lock (padlock)
                {
                    return _instance ??= new OptionsBuilder();
                }
            }
        }

        public DbContextOptions<ApplicationDbContext> Options => _options.Options;
    }
}
