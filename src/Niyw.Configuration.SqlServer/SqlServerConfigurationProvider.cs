using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Niyw.Configuration.SqlServer {
    public class SqlServerConfigurationProvider : ConfigurationProvider {
        private readonly DbContextOptions<AppConfigDbContext> _dbOptions;
        private readonly Timer _reloadTimer = new Timer();
        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

        public SqlServerConfigurationProvider(Action<DbContextOptionsBuilder> dbOptionsAction) {
            var builder = new DbContextOptionsBuilder<AppConfigDbContext>();
            dbOptionsAction(builder);
            _dbOptions = builder.Options;
            _reloadTimer.AutoReset = false;
            _reloadTimer.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
            _reloadTimer.Elapsed += (s, e) => { OnReload(); };
            ChangeToken.OnChange(() => _reloadToken, Load);
        }

        public override void Load() {
            try {
                using (var db = new AppConfigDbContext(_dbOptions)) {
                    Data.Clear();
                    Data = !db.KeyValues.Any()
                   ? new Dictionary<string, string>()
                   : db.KeyValues.AsNoTracking().ToDictionary(c => c.KeyName, c => c.KeyValue);
                }
            }
            finally {
                _reloadTimer.Start();
            }
        }

        protected new void OnReload() {
            var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }
    }
}
