using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Niyw.Configuration.SqlServer {
    public class SqlCacheChangeToken : IChangeToken {
        public bool HasChanged => throw new NotImplementedException();

        public bool ActiveChangeCallbacks => throw new NotImplementedException();

        public IDisposable RegisterChangeCallback(Action<object> callback, object state) {
            throw new NotImplementedException();
        }
    }
}
