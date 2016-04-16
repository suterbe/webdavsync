using System;
using System.Collections.Generic;
using System.Text;

namespace WebDavSync.Types {
    /// <summary>
    /// Klasse um EventParameter von Events zu verarbeiten
    /// </summary>
    public class LogEventArgs : EventArgs {

        /// <summary>
        /// Konstruktor für EventArgs
        /// </summary>
        /// <param name="logtext">Nimmt den LogText als Sring entgegen</param>
        public LogEventArgs(string logtext) {
            this.Log = logtext;
        }

        /// <summary>
        /// Konstruktor für EventArgs
        /// </summary>
        /// <param name="logtext">Nimmt den LogText als Sring entgegen</param>
        public LogEventArgs(string logtext, params object[] args)
            : this(string.Format(logtext, args)) {
        }

        /// <summary>
        /// LogText als String
        /// </summary>
        public string Log { get; private set; }
    }
}
