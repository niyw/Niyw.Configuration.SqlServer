using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;

namespace Niyw.Configuration.SqlServer {
    public class SqlServerConfigurationSource: IConfigurationSource {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;
        public bool ReloadOnChange { get; set; } = true;
        public SqlServerConfigurationSource(Action<DbContextOptionsBuilder> optionsAction) {
            _optionsAction = optionsAction;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) {
            return new SqlServerConfigurationProvider(_optionsAction);           
        }
    }
}
