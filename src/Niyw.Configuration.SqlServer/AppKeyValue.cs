using System;

namespace Niyw.Configuration.SqlServer {
    public class AppKeyValueItem {        
        public string AppId { get; set; }
        public string EnvironmentName { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
        public byte[] RowVersion { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
