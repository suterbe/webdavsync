using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using System.IO;
using System.Threading;
using WebDavSync.Core;
using WebDavSync.Client;
using WebDavSync.UnitTests.Infrastructure;

namespace WebDavSync.UnitTests.Core {

    [TestFixture]
    public class WebDavServiceTests : MockTestBase<WebDavService> {
        
        private IWebDavClient WebDavClientMock { get; set; }

        private string DownloadDirectory {
            get { return Path.Combine(Environment.CurrentDirectory, "Downloads"); }
        }

        protected override WebDavService CreateSut() {
            this.WebDavClientMock = this.MockFactory.StrictMock<IWebDavClient>();

            var sut = new WebDavService(this.WebDavClientMock, this.DownloadDirectory, "UnitTest");

            // Logging auf die Testkonsole umleiten
            sut.LogData += (sender, e) => {
                Console.WriteLine(e.Log);
            };

            sut.SyncProfileEnd += (sender, e) => {
                Console.WriteLine(e.Status);
            };

            return sut;
        }

        [TearDown]
        public void Teardown() {
            Directory.Delete(this.DownloadDirectory, true);
        }

        [Test]
        public void Download_all_content_with_nothing_to_downlaod() {
            var webDavServerPath = "Files";
            var contentList = new List<WebDavContent>();
            var statusCode = 207;

            var syncHandle = new AutoResetEvent(false);

            var listCompleteRaiser = this.WebDavClientMock.GetEventRaiser(w => w.ListComplete += null);

            // Verzögertes auslösen des Events.. 
            System.Threading.ThreadPool.QueueUserWorkItem((o) => {
                syncHandle.WaitOne();
                listCompleteRaiser.Raise(contentList, statusCode);
            });

            With.Mocks(this.MockFactory).Expecting(delegate {
                Expect.Call(this.WebDavClientMock.WebDavServerPath)
                    .Return(webDavServerPath);

                Expect.Call(() => this.WebDavClientMock.List(webDavServerPath))
                    .Do((Action<string>)delegate(string path) {
                        syncHandle.Set();
                    });
            }).Verify(delegate {
                this.Sut.DownloadWebDavContent();
            });

            Assert.IsTrue(Directory.Exists(this.DownloadDirectory));
        }

        [Test]
        public void Download_all_content_having_some_files_with_a_flat_structure_on_the_server() {
            var webDavServerPath = "Files";
            var contentList = new List<WebDavContent> {
                new WebDavContent { 
                    FilePath = @"Files/Readme.txt",
                    Contentlength = 10,
                    Creationdate = "2011-05-04 12:32:54",
                    Lastmodified = "2011-05-04 15:47:08",
                    Contenttype = "text/plain"
                },
                new WebDavContent { 
                    FilePath = @"Files/Gurkenmenü.txt",
                    Contentlength = 12,
                    Creationdate = "2011-05-04 12:33:02",
                    Lastmodified = "2011-05-04 12:33:02",
                    Contenttype = "text/plain"
                }
            };
            var statusCode = 207;

            var syncHandle = new AutoResetEvent(false);

            var listCompleteRaiser = this.WebDavClientMock.GetEventRaiser(w => w.ListComplete += null);
            var downloadCompleteRaiser = this.WebDavClientMock.GetEventRaiser(w => w.DownloadComplete += null);

            // Verzögertes auslösen des Events.. 
            System.Threading.ThreadPool.QueueUserWorkItem((o) => {
                syncHandle.WaitOne();
                listCompleteRaiser.Raise(contentList, statusCode);
                
                syncHandle.WaitOne();
                downloadCompleteRaiser.Raise("Readme.txt", 200);

                syncHandle.WaitOne();
                downloadCompleteRaiser.Raise("Gurkenmenü.txt", 200);                
            });

            With.Mocks(this.MockFactory).Expecting(delegate {
                Expect.Call(this.WebDavClientMock.WebDavServerPath)
                    .Return(webDavServerPath);

                Expect.Call(() => this.WebDavClientMock.List(webDavServerPath))
                    .Do((Action<string>)delegate(string path) {
                        syncHandle.Set();
                    });

                Expect.Call(() => this.WebDavClientMock.Download("Files/Readme.txt",
                        Path.Combine(this.DownloadDirectory, "Readme.txt")))
                    .Do((Action<string, string>)delegate(string remotePath, string localPath) {
                        syncHandle.Set();
                    });

                Expect.Call(() => this.WebDavClientMock.Download("Files/Gurkenmenü.txt",
                        Path.Combine(this.DownloadDirectory, "Gurkenmenü.txt")))
                    .Do((Action<string, string>)delegate(string remotePath, string localPath) {
                        syncHandle.Set();
                    });

            }).Verify(delegate {
                this.Sut.DownloadWebDavContent();
            });

            Assert.IsTrue(Directory.Exists(this.DownloadDirectory));
        }

        [Test]
        public void Download_all_content_having_a_file_in_a_new_folder_on_the_server() {
            var webDavServerPath = "Files";
            var statusCode = 207;
            var syncHandle = new AutoResetEvent(false);

            var listCompleteRaiser = this.WebDavClientMock.GetEventRaiser(w => w.ListComplete += null);
            var downloadCompleteRaiser = this.WebDavClientMock.GetEventRaiser(w => w.DownloadComplete += null);

            // Verzögertes auslösen des Events.. 
            System.Threading.ThreadPool.QueueUserWorkItem((o) => {
                syncHandle.WaitOne();
                listCompleteRaiser.Raise(
                    new List<WebDavContent> {
                        new WebDavContent { 
                            FilePath = @"Files/Folder1",
                            Contentlength = 0,
                            Creationdate = "2011-05-04 12:32:54",
                            Lastmodified = "2011-05-04 15:47:08",
                            Contenttype = "text/plain"
                        }
                    }, statusCode);

                syncHandle.WaitOne();
                listCompleteRaiser.Raise(
                    new List<WebDavContent> {
                        new WebDavContent { 
                            FilePath = @"Files/Folder1/Readme.txt",
                            Contentlength = 10,
                            Creationdate = "2011-05-04 12:32:56",
                            Lastmodified = "2011-05-04 15:47:10",
                            Contenttype = "text/plain"
                        }
                    }, statusCode);

                syncHandle.WaitOne();
                downloadCompleteRaiser.Raise("Readme.txt", 200);
            });

            With.Mocks(this.MockFactory).Expecting(delegate {
                Expect.Call(this.WebDavClientMock.WebDavServerPath)
                    .Return(webDavServerPath);

                Expect.Call(() => this.WebDavClientMock.List(webDavServerPath))
                    .Do((Action<string>)delegate(string path) {
                        syncHandle.Set();
                    });

                Expect.Call(() => this.WebDavClientMock.List(string.Format(@"{0}/{1}",webDavServerPath, "Folder1")))
                    .Do((Action<string>)delegate(string path) {
                        syncHandle.Set();
                    });

                Expect.Call(() => this.WebDavClientMock.Download("Files/Folder1/Readme.txt", 
                        Path.Combine(this.DownloadDirectory, Path.Combine("Folder1", "Readme.txt"))))
                    .Do((Action<string, string>)delegate(string remotePath, string localPath) {
                        syncHandle.Set();
                    });
            }).Verify(delegate {
                this.Sut.DownloadWebDavContent();
            });

            Assert.IsTrue(Directory.Exists(this.DownloadDirectory));
            Assert.IsTrue(Directory.Exists(Path.Combine(this.DownloadDirectory, "Folder1")));
        }

        [Test]
        public void Download_all_content_having_no_files_to_update_on_the_server() {
            var webDavServerPath = "Files";
            var contentList = new List<WebDavContent> {
                new WebDavContent { 
                    FilePath = @"Files/AlreadyPresent.txt",
                    Contentlength = 10,
                    Creationdate = DateTime.Now.AddMinutes(-1).ToString(),
                    Lastmodified = DateTime.Now.AddMinutes(-2).ToString(),
                    Contenttype = "text/plain"
                }
            };
            var statusCode = 207;

            Directory.CreateDirectory(this.DownloadDirectory);
            File.WriteAllText(Path.Combine(this.DownloadDirectory, "AlreadyPresent.txt"),
                "8====D~~ {()} yes/no?");

            var syncHandle = new AutoResetEvent(false);

            var listCompleteRaiser = this.WebDavClientMock.GetEventRaiser(w => w.ListComplete += null);

            // Verzögertes auslösen des Events.. 
            System.Threading.ThreadPool.QueueUserWorkItem((o) => {
                syncHandle.WaitOne();
                listCompleteRaiser.Raise(contentList, statusCode);
            });

            With.Mocks(this.MockFactory).Expecting(delegate {
                Expect.Call(this.WebDavClientMock.WebDavServerPath)
                    .Return(webDavServerPath);

                Expect.Call(() => this.WebDavClientMock.List(webDavServerPath))
                    .Do((Action<string>)delegate(string path) {
                        syncHandle.Set();
                    });
            }).Verify(delegate {
                this.Sut.DownloadWebDavContent();
            });

            Assert.IsTrue(Directory.Exists(this.DownloadDirectory));
        }       
    }
}