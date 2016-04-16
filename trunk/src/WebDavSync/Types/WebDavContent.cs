using System;
using System.Collections.Generic;
using System.Text;

namespace WebDavSync {
    public class WebDavContent {
        public string FilePath { get; set; }
        public string Lastmodified { get; set; }
        public string Contenttype { get; set; }
        public string Creationdate { get; set; }
        public int Contentlength { get; set; }
    }
}
