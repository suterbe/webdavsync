using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WebDavSync.UnitTests.Server {
    public class RequestParser {

        public RequestParser(StreamReader requestStream) {
            this.RequestStream = requestStream;
        }

        private StreamReader RequestStream { get; set; }

        public bool IsValid() {
            return true;
        }

        public string CreateResponse() {
            var requestString = GetRequestString();

            // GET /Home/Readme.txt HTTP/1.1
//Content-Type: text/xml
//Host: localhost:8394
//Connection: Keep-Alive

            if (requestString.Contains("PROPFIND /Home") && requestString.Contains("Depth: 2")) {
                return LoadResponeFromFile(@"Infrastructure\Server\Responses\List_Home_Depth2.xml");
            } else if (requestString.Contains("PROPRFIND /Home") && requestString.Contains("Depth: 1") || requestString.Contains("PROPFIND /Home") || requestString.Contains("PROPFIND /")) {
                return LoadResponeFromFile(@"Infrastructure\Server\Responses\List_Home_Depth1.xml");
            } else if (requestString.Contains("GET /Home/Readme.txt")) {
                return "Inhalt von Readme";
            } else {
                throw new InvalidOperationException("unhandled request string.");
            }
        }

        private string GetRequestString() {
            var requestStringBuilder = new StringBuilder();

            var requestLine = string.Empty;
            while ((requestLine = this.RequestStream.ReadLine()) != null && requestLine != "") {
                requestStringBuilder.AppendLine(requestLine);
            }

            var requestString = requestStringBuilder.ToString();
            return requestString;
        }

        private string LoadResponeFromFile(string path) {
            return File.ReadAllText(Path.Combine(Environment.CurrentDirectory, path), Encoding.UTF8);
        }
    }
}
