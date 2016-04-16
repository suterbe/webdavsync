using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using WebDavSync.UnitTests.Executor;
using System.Net;
using WebDavSync.UnitTests.Queue;

namespace WebDavSync.UnitTests.Server {
    internal class WebDavServer {
        private Boolean running = true;

        private int Port { get; set; }
        private string Host { get; set; }
        private Thread ServerThread { get; set; }

        private IExecutor Executor { get; set; }
        private Socket ServerSocket { get; set; }

        public void Start(int port, string host) {
            this.ServerThread = new Thread(Run);
        
            this.Port = port;
            this.Host = host;

            this.ServerThread.IsBackground = true;

            this.ServerThread.Start();
        }

        public void Stop() {
            running = false;

            if (!this.ServerThread.Join(TimeSpan.FromMilliseconds(500))) {
                this.ServerThread.Abort();

                this.ServerThread.Join();
            }            
        }

        private IExecutor CreateExecutor() {
            var queue = new BoundedBufferWithSemaphor(3);
            return new PlainWorkerPool(queue, 1);
        }

        #region Server Thread

        private void Run() {
            try {
                this.Executor = CreateExecutor();
                this.ServerSocket = CreateServerSocket();                

                this.ServerSocket.BeginAccept(AcceptClient, null);

                while (running && this.ServerSocket.IsBound) {
                    Thread.Sleep(TimeSpan.FromMilliseconds(250));
                }
            } catch (ThreadAbortException) {
            } finally {
                if (this.ServerSocket != null) {
                    this.ServerSocket.Close();
                }
                if (this.Executor != null) {
                    this.Executor.Dispose();
                }
            }
        }

        private void AcceptClient(IAsyncResult ar) {
            if (running) {
                var client = this.ServerSocket.EndAccept(ar);

                this.ServerSocket.BeginAccept(AcceptClient, null);

                var handler = CreateHandler(client);
                this.Executor.Execute(handler.Run);
            }
        }

        private Socket CreateServerSocket() {
            IPAddress ipAddress = Dns.GetHostEntry(Host).AddressList[0];
            TcpListener listener = new TcpListener(ipAddress, Port);
            listener.Start();

            listener.Server.SendBufferSize = 6550000;

            return listener.Server;
        }

        private WebDavHandler CreateHandler(Socket client) {
            return new WebDavHandler(client);
        } 

        #endregion

        
    }
}
