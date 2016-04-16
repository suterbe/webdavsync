using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace WebDavSync.UnitTests.Server {

    internal class WebDavHandler {
        private Socket Client { get; set; }
        private StreamReader Request { get; set; }
        private StreamWriter Response { get; set; }

        private RequestParser Parser { get; set; }

        public WebDavHandler(Socket client) {
            this.Client = client;

            var networkStream = new NetworkStream(client, true);

            this.Request = new StreamReader(networkStream);
            this.Response = new StreamWriter(networkStream);
        }

        public void Run() {
            try {
                if (this.ReadRequest()) {
                    this.CreateResponse();
                }
                this.Client.Close();
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            }
        }

        private bool ReadRequest() {
            this.Parser = new RequestParser(this.Request);

            return this.Parser.IsValid();
        }

        private void CreateResponse() {
            WriteResult("HTTP/1.1 200 OK");
            WriteResult("Server: jac's WebDav Server 0.1");

            var responseString = this.Parser.CreateResponse();
                        
            WriteResult("Content-Length: " + responseString.Length);
            WriteResult(""); // Wichtig!!
            WriteResult(responseString);
        }

        private void WriteResult(string message) {
            //Console.WriteLine("> " + message);
            this.Response.Write(message + "\r\n");
            this.Response.Flush();
        }

        private void WriteError(int status, string message) {
            string output = "<h1>HTTP/1.0 " + status + " " + message + "</h1>";
            Console.WriteLine("> " + status.ToString() + " " + message);
            this.Response.Write(output + "\r\n");
            this.Response.Flush();
        }
    }
}
