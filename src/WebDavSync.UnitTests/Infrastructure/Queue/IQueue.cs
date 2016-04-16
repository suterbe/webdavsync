using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebDavSync.UnitTests.Queue {
    internal interface IQueue {
        void Enqueue(Object x);
        Object Dequeue();
    }
}
