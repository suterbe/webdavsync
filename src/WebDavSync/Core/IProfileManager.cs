using System;
using System.Collections.Generic;
namespace WebDavSync.Core {
    public interface IProfileManager {
        List<WebDAVClientProfile> GetProfiles();
        void SaveProfiles(List<WebDAVClientProfile> listProfiles);
    }
}
