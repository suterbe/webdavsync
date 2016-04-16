using System;
using System.Collections.Generic;
using System.Text;
using WebDavSync.Core;

namespace WebDavSync.Types {

    /// <summary>
    /// Klasse um EventParameter von Events zu verarbeiten
    /// </summary>
    public class SyncStatusEventArgs : EventArgs {

        /// <summary>
        /// Konstruktor für EventArgs
        /// </summary>
        /// <param name="logtext">Nimmt den LogText als Sring entgegen</param>
        public SyncStatusEventArgs(SyncStatus status) {
            this.Status = status;
        }

        /// <summary>
        /// Status als boolean
        /// </summary>
        public SyncStatus Status { get; private set; }
    }
}
