using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WebDavSync.UnitTests.Executor {
    internal interface IExecutor : IDisposable {
        void Execute(ThreadStart threadStart);
    }
}
