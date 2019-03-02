using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Niyw.Configuration.SqlServer {
    public class SqlServerConfigurationProvider : ConfigurationProvider,IDisposable {
        private readonly DbContextOptions<AppConfigDbContext> _dbOptions;
        private readonly Timer _reloadTimer = null;
        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();
        private SqlServerConfigurationSource _source = null;
        private volatile string LastVersion = string.Empty;
        public SqlServerConfigurationProvider(Action<DbContextOptionsBuilder> dbOptionsAction, SqlServerConfigurationSource source) {
            _source = source;
            var builder = new DbContextOptionsBuilder<AppConfigDbContext>();
            dbOptionsAction(builder);
            _dbOptions = builder.Options;

            if (_source.ReloadOnChange) {
                _reloadTimer = new Timer();
                _reloadTimer.AutoReset = false;
                _reloadTimer.Interval = TimeSpan.FromSeconds(_source.RefreshInterval).TotalMilliseconds;
                _reloadTimer.Elapsed += (s, e) => { OnReload(); };
                ChangeToken.OnChange(() => _reloadToken, Load);
            }
        }
               
        public override void Load() {
            try {
                using (var configDbContext = new AppConfigDbContext(_dbOptions)) {

                    var lastVersion = configDbContext.GetSum().Result;
                    if (LastVersion == lastVersion)
                        return;
                    LastVersion = lastVersion;

                    Data.Clear();
                    Data = !configDbContext.KeyValues.Any()
                   ? new Dictionary<string, string>()
                   : configDbContext.KeyValues.AsNoTracking().ToDictionary(c => c.KeyName, c => c.KeyValue);
                }
            }
            finally {
                _reloadTimer?.Start();
            }
        }

        protected new void OnReload() {
            if (_source.ReloadOnChange) {
                var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
                previousToken.OnReload();
            }
        }

        public void Dispose() {
            _reloadTimer?.Dispose();
        }
    }
}
