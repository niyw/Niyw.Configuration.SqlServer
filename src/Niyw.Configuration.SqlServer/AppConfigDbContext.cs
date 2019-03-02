using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Niyw.Configuration.SqlServer {
    public class AppConfigDbContext : DbContext {
        private const string ConfigTableName = "Tb_AppSettings";
        public static string Sql_CheckSum = $"SELECT CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM {ConfigTableName} WITH (NOLOCK);";
        public DbSet<AppKeyValueItem> KeyValues { get; set; }
        public AppConfigDbContext(DbContextOptions options) : base(options) {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<AppKeyValueItem>().ToTable(ConfigTableName)
                .HasKey(k => new { k.AppId, k.KeyName });
            modelBuilder.Entity<AppKeyValueItem>().Property(p => p.AppId).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<AppKeyValueItem>().Property(p => p.KeyName).IsRequired().HasColumnName("ItemKey").HasMaxLength(4000);
            modelBuilder.Entity<AppKeyValueItem>().Property(p => p.EnvironmentName).IsRequired().HasDefaultValue("Dev").HasMaxLength(20);
            modelBuilder.Entity<AppKeyValueItem>().Property(p => p.KeyValue).HasColumnName("ItemValue");
            modelBuilder.Entity<AppKeyValueItem>().Property(p => p.RowVersion).IsRowVersion();
            modelBuilder.Entity<AppKeyValueItem>().Property(p => p.LastUpdate).IsRequired().HasDefaultValue(DateTime.Now);
        }
        public async Task<string> GetSum() {
            var conn = Database.GetDbConnection();
            await conn.OpenAsync();
            try {
                var command = conn.CreateCommand();
                command.CommandText = Sql_CheckSum;
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync()) {
                    return Convert.ToString(reader[0]);
                }
                return string.Empty;
            }
            finally {
                conn.Close();
            }
        }
    }
}
