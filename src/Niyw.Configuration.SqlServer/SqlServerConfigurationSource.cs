using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Niyw.Configuration.SqlServer {
    public class SqlServerConfigurationSource: IConfigurationSource {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;
        public bool ReloadOnChange { get; private set; } = true;
        public int RefreshInterval { get; private set; } = 5;
        public SqlServerConfigurationSource(Action<DbContextOptionsBuilder> optionsAction, bool reloadOnChange=true,int refreshInterval=5) {            
            _optionsAction = optionsAction;
            ReloadOnChange = reloadOnChange;
            if (refreshInterval < 5)
                refreshInterval = 5;
            RefreshInterval = refreshInterval;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) {
            return new SqlServerConfigurationProvider(_optionsAction,this);           
        }
    }
}
