using System;
using System.Collections.Generic;
using System.Text;

namespace WebDavSync {

    public class WebDAVClientProfile {
        public string DavServerPath { get; set; }
        public string DavServer { get; set; }
        public string DavUser { get; set; }
        public string DavPass { get; set; }
        public string LocalPath { get; set; }
        public bool OverrideItems { get; set; }
        public string ProfileName { get; set; }
    }
}