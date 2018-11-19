using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Niyw.Configuration.SqlServer {
    public class SqlCacheChangeToken : IChangeToken {
        private Action _callback;
        public bool HasChanged => true;
        //public bool HasChanged =>_cts.IsCancellationRequested;

        public bool ActiveChangeCallbacks => true;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state) {
            //throw new NotImplementedException();
            _callback = () => callback(state);
            return null;
        }
    }
}
