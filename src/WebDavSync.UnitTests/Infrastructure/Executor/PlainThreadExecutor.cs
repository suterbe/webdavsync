using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WebDavSync.UnitTests.Executor {
    public class PlainThreadExecutor : IExecutor {
        public void Execute(ThreadStart threadStart) {
            new Thread(threadStart).Start();
        }

        public void Dispose() {
            
        }
    }
}
