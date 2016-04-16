using System;
namespace WebDavSync.Core {
    public interface ICryptoService {
        string Decrypt(string value);
        string Encrypt(string text);
    }
}
