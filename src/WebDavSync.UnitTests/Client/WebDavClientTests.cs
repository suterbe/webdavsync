using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using WebDavSync.UnitTests.Server;
using System.IO;
using WebDavSync.Core;
using WebDavSync.Client;

namespace WebDavSync.UnitTests.Client {
    
    [TestFixture]
    public class WebDavClientTests {

        // Todo,jac: Tests für den Fehlerfall (HttpErrorCode e.g. invalid credentials)

        private WebDavClient Sut { get; set; }
        private WebDavServer ServerStub { get; set; }

        private string DownloadDirectory {
            get { return Path.Combine(Environment.CurrentDirectory, "Downloads"); }
        }

        [TestFixtureSetUp]
        public void SetUpTest() {
            // start web dav testserver
            this.ServerStub = new WebDavServer();
            this.ServerStub.Start(8394, "localhost");

            // Zeit geben zum starten..
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            if (this.ServerStub != null) {
                this.ServerStub.Stop();
            }
        }

        [SetUp]
        public void SetUp() {
            this.Sut = new WebDavClient(
                new WebDAVClientProfile {
                    DavServer = @"http://localhost:8394",
                    DavServerPath = string.Empty,
                    DavUser = "john.doe@google.com",
                    DavPass = "nÖ842jä$ownlI1"
                });

            // Sicherstellen, dass Downloaddirectory existiert
            if (!Directory.Exists(this.DownloadDirectory)) {
                Directory.CreateDirectory(this.DownloadDirectory);
            }
        }

        [TearDown]
        public void TearDown() {
            // Downloads lösschen
            Directory.Delete(this.DownloadDirectory, true);

            // Kurz warten (Hilft bzgl. Netzwerkbezogene (Stream closed shizzle))
            System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public void List_content_without_any_preferences() {
            var signal = new System.Threading.AutoResetEvent(false);

            this.Sut.ListComplete += (contentList, statusCode) => {
                DumpContentList(contentList);
                Assert.AreEqual(2, contentList.Count);
                signal.Set();
            };

            this.Sut.List();

            if (!signal.WaitOne(TimeSpan.FromSeconds(2))) {
                Assert.Fail("Timeout reached");
            }
        }

        [Test]
        public void List_content_for_a_given_path() {
            var signal = new System.Threading.AutoResetEvent(false);

            this.Sut.ListComplete += (contentList, statusCode) => {
                DumpContentList(contentList);
                Assert.AreEqual(2, contentList.Count);
                signal.Set();
            };

            this.Sut.List("Home");

            if (!signal.WaitOne(TimeSpan.FromSeconds(2))) {
                Assert.Fail("Timeout reached");
            }
        }
        [Test]
        public void List_content_for_a_given_path_and_depth() {
            var signal = new System.Threading.AutoResetEvent(false);

            this.Sut.ListComplete += (contentList, statusCode) => {
                DumpContentList(contentList);
                Assert.AreEqual(4, contentList.Count);
                signal.Set();
            };

            this.Sut.List("Home", 2);

            if (!signal.WaitOne(TimeSpan.FromSeconds(2))) {
                Assert.Fail("Timeout reached");
            }
        }

        [Test]
        public void Download_a_given_file_to_a_given_location() {
            var requestedFile = @"Home\Readme.txt";
            var localPath = Path.Combine(this.DownloadDirectory, requestedFile);

            var signal = new System.Threading.AutoResetEvent(false);

            this.Sut.DownloadComplete += (downloadedFile, statusCode) => {
                Assert.AreEqual(200, statusCode);
                Assert.IsTrue(File.Exists(localPath), "Datei wurde nicht 'heruntergeladen'.");
                Assert.AreEqual("Inhalt von Readme", File.ReadAllText(localPath), "Inhalt der 'heruntergeladenen' Datei ist nicht korrekt.");

                signal.Set();
            };

            Directory.CreateDirectory(Path.Combine(this.DownloadDirectory, @"Home"));

            this.Sut.Download(requestedFile,
                Path.Combine(this.DownloadDirectory, requestedFile));

            if (!signal.WaitOne(TimeSpan.FromSeconds(2))) {
                Assert.Fail("Timeout reached");
            }
        }

        private static void DumpContentList(IList<WebDavContent> contentList) {
            Console.WriteLine("Files auf dem Server:");

            foreach (var item in contentList) {
                Console.WriteLine(" - " + item.FilePath);
            }

            Console.WriteLine();
        }
    }
}
