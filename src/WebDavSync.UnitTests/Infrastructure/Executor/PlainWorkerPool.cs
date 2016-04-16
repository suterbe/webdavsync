using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WebDavSync.UnitTests.Queue;

namespace WebDavSync.UnitTests.Executor {
    internal class PlainWorkerPool : IExecutor {
        protected IQueue workQueue;
        protected Thread[] workerArray;
        private bool busy = true;

        public PlainWorkerPool(IQueue workQueue, int nWorkers) {
            this.workQueue = workQueue;
            workerArray = new Thread[nWorkers];
            for (int i = 0; i < nWorkers; ++i)
                activate(i);
        }

        public void Execute(ThreadStart threadStart) {
            workQueue.Enqueue(threadStart);
        }

        protected void activate(int i) {
            Thread runLoop = new Thread(delegate() {
                try {
                    while (busy) {
                        ThreadStart threadStart = (ThreadStart)workQueue.Dequeue();
                        threadStart.Invoke();
                    }
                } catch (ThreadAbortException) {
                }
            });
            workerArray[i] = runLoop;
            runLoop.Start();
        }

        public void ShutDown() {
            busy = false;
            for (int i = 0; i < workerArray.Length; i++) {
                Execute(new ThreadStart(delegate() { }));
            }
        }

        public void Dispose() {
            // Alle Threads beenden
            if (workerArray != null) {
                foreach (var worker in workerArray) {
                    if (worker != null) {
                        worker.Abort();
                    }
                }
            }
        }
    }
}
