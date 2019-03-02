using Microsoft.Extensions.Primitives;
using System;
using System.Data.SqlClient;

namespace Niyw.Configuration.SqlServer {

    public class SqlCacheChangeToken : IChangeToken {
        private string _connection;
        public SqlCacheChangeToken(string connection) {
            _connection = connection;
        }
        public bool ActiveChangeCallbacks => true;
        private string LastSum = string.Empty;
        public bool HasChanged {
            get {
                var query = AppConfigDbContext.Sql_CheckSum;
                try {
                    using (var conn = new SqlConnection(_connection))
                    using (var cmd = new SqlCommand(query, conn)) {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader()) {
                            var tVal = string.Empty;
                            while (reader.Read()) {
                                tVal = Convert.ToString(reader[0]);
                            }
                            if (tVal == LastSum)
                                return true;
                            tVal = LastSum;
                            return false;
                        }
                    }

                }
                catch (Exception) {
                    return false;
                }
            }
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => EmptyDisposable.Instance;
    }

    internal class EmptyDisposable : IDisposable {
        public static EmptyDisposable Instance { get; } = new EmptyDisposable();
        private EmptyDisposable() { }
        public void Dispose() { }
    }
}


