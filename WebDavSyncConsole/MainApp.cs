using System;
using System.Collections.Generic;
using System.Text;
using WebDavSync;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WebDavSyncConsole
{
    class MainApp
    {
        private WebDAVClient webDavClient = new WebDAVClient();
        private Properties.Settings webDavClientSettings = new WebDavSyncConsole.Properties.Settings();

        public MainApp(string[] args)
        {
            Debug.Listeners.Add(new ConsoleTraceListener());


            webDavClient.WebDavServerUrl = webDavClientSettings.WebDavServerUrl;
            webDavClient.WebDavServerPath = webDavClientSettings.WebDavServerPath;
            webDavClient.WebDavUsername = webDavClientSettings.WebDavServerUsername;
            webDavClient.WebDavPassword = webDavClientSettings.WebDavServerPassword;

            start();


        }

        private void start()
        {
            Console.WriteLine("WebDavSyncConsole Version 0.0.1 Beta");
            Console.WriteLine("(c) Benjamin Suter, 2009" + Environment.NewLine);

            Console.WriteLine("What would you like to do?" + Environment.NewLine +
                " 0 - Exit application" + Environment.NewLine +
                " 1 - Show Configuration" + Environment.NewLine +
                " 2 - Download WebDav Content to local Folder" + Environment.NewLine
            );
            int choise = 0;
            while (true)
            {

                try
                {
                    choise = Convert.ToInt32(Console.ReadLine());
                }
                catch { }

                switch (choise)
                {
                    case 0:
                        return;

                    case 1:
                        showSettings();
                        break;
                    case 2:
                        downloadWebDavContent(webDavClientSettings.WebDavServerPath, webDavClientSettings.LocalSyncPath);
                        break;
                    default:
                        Console.WriteLine("ungültie Wahl");
                        break;
                }
            }
        }

        private void downloadWebDavContent(string currentWebDavPath, string currentLocalPath)
        {
            _webDavContentList.Clear();
            if (!Directory.Exists(currentLocalPath)){
                try { Directory.CreateDirectory(currentLocalPath); }
                catch (Exception ex)
                {
                    Debug.WriteLine("Fehler beim erstellen des Ordners " + currentLocalPath);
                }}
            autoResetEvent = new AutoResetEvent(false);
            webDavClient.ListComplete += new ListCompleteDel(c_ListComplete);
            webDavClient.List(currentWebDavPath);
            autoResetEvent.WaitOne();
            List<WebDavContent> contentList = new List<WebDavContent>(_webDavContentList);
            foreach (WebDavContent item in contentList)
            {
                string fileName = item.FilePath;
                if (item.Contentlength == 0)
                //if (fileName.EndsWith("/")) //Folder
                {
                    downloadWebDavContent(fileName, currentLocalPath + "\\" + fileName.Substring(fileName.TrimEnd(new char[] { '/' }).LastIndexOf('/')).Trim('/'));
                }
                else //File
                {
                    string strlocalfile = currentLocalPath + "\\" + fileName.Substring(fileName.LastIndexOf('/')).TrimStart('/');
                    try
                    {
                        FileInfo objLocalFile = new FileInfo(strlocalfile);
                        if (objLocalFile.LastWriteTime > Convert.ToDateTime(item.Lastmodified))
                        {
                            //File ist lokal neuer
                            Debug.WriteLine("Lokale Datei ist neuer --> kein Download");
                        }
                        else
                        {
                            Debug.WriteLine("Lokale Datei ist älter --> Download");
                            throw new Exception("Datei ist älter, muss heruntergeladen werden");
                        }

                    }
                    catch
                    {
                        autoResetEvent = new AutoResetEvent(false);
                        webDavClient.DownloadComplete += new DownloadCompleteDel(c_DownloadComplete);
                        webDavClient.Download(fileName, strlocalfile);
                        autoResetEvent.WaitOne();
                    }


                }
            }
        }

        private void showSettings()
        {
            Console.WriteLine(webDavClient.WebDavServerUrl);
        }

        #region Statische Methoden
        static AutoResetEvent autoResetEvent;




        static void c_DeleteComplete(int statusCode)
        {
            Debug.Assert(statusCode == 204);
            autoResetEvent.Set();
        }

        static void c_UploadComplete(int statusCode, object state)
        {
            Debug.Assert(statusCode == 201);
            autoResetEvent.Set();
        }

        static void c_CreateDirComplete(int statusCode)
        {
            Debug.Assert(statusCode == 200 || statusCode == 201);
            autoResetEvent.Set();
        }

        static List<WebDavContent> _webDavContentList = new List<WebDavContent>();
        static void c_ListComplete(List<WebDavContent> list, int statusCode)
        {
            Debug.Assert(statusCode == 207);
            _webDavContentList = list;
            autoResetEvent.Set();
        }

        static void c_DownloadComplete(int code)
        {
            Debug.Assert(code == 200);
            autoResetEvent.Set();
        }
        #endregion
    }
}
