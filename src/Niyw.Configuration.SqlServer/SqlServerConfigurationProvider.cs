using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Niyw.Configuration.SqlServer {
    public class SqlServerConfigurationProvider : ConfigurationProvider {

        private Action<DbContextOptionsBuilder> OptionsAction { get; }
        public SqlServerConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction) {
            OptionsAction = optionsAction;            

            ChangeToken.OnChange(
                () => new SqlCacheChangeToken(),
                () => {
                    System.Threading.Thread.Sleep(5000);
                    LoadFromSql();
                }
                );
        }

        // Load config data from EF DB.
        public override void Load() {
            LoadFromSql();
        }
        private void LoadFromSql() {
            var builder = new DbContextOptionsBuilder<AppConfigDbContext>();
            OptionsAction(builder);
            using (var dbContext = new AppConfigDbContext(builder.Options)) {
                dbContext.Database.EnsureCreated();
                Data = !dbContext.KeyValues.Any()
                    ? new Dictionary<string, string>()
                    : dbContext.KeyValues.ToDictionary(c => c.Id, c => c.Value);
            }
        }
    }
}
