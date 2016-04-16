using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace WebDavSync.Core {
    public class CryptoService : ICryptoService {
        private const string ENCRYPTIONKEY = "ZrOORX411JqD4a46vOvrIIbnvyB5Xxkk";
        private const string VECTOR = "AJIhPpKhiqt=";
        
        public CryptoService() {
            this.CryptoAlgorithm = new TripleDESCryptoServiceProvider();
            this.CryptoAlgorithm.Key = Convert.FromBase64String(ENCRYPTIONKEY);
            this.CryptoAlgorithm.IV = Convert.FromBase64String(VECTOR);
        }

        private SymmetricAlgorithm CryptoAlgorithm { get; set; }
        private Encoding EncodingProvider {
            get { return Encoding.UTF8; }
        }

        /// <summary>
        /// Verschlüsselt einen Text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Verschlüsselter Text</returns>
        public string Encrypt(string text) {
            var encrypted = string.Empty;

            if (!string.IsNullOrEmpty(text)) {
                ICryptoTransform ct = null;
                MemoryStream ms = null;

                try {
                    var buffer = this.EncodingProvider.GetBytes(text);

                    ct = CryptoAlgorithm.CreateEncryptor(CryptoAlgorithm.Key, CryptoAlgorithm.IV);
                    ms = new MemoryStream();

                    using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write)) {
                        cs.Write(buffer, 0, buffer.Length);
                        cs.FlushFinalBlock();
                    }

                    encrypted = Convert.ToBase64String(ms.ToArray());
                } catch (Exception) {
                } finally {
                    if (ms != null) {
                        ms.Close();
                        ms.Dispose();
                    }
                    if (ct != null) {
                        ct.Dispose();
                    }
                }
            }

            return encrypted;
        }

        /// <summary>
        /// Entschlüsselt einen Verschlüsselten String
        /// </summary>
        /// <param name="value">Verschlüsselter String</param>
        /// <returns>Unverschlüsselter Text</returns>
        public string Decrypt(string value) {
            var decrypted = string.Empty;

            if (!string.IsNullOrEmpty(value)) {
                ICryptoTransform ct = null;
                MemoryStream ms = null;

                try {
                    var buffer = Convert.FromBase64String(value);

                    ct = CryptoAlgorithm.CreateDecryptor(CryptoAlgorithm.Key, CryptoAlgorithm.IV);
                    ms = new MemoryStream();
                    
                    using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write)) {
                        cs.Write(buffer, 0, buffer.Length);
                        cs.FlushFinalBlock();
                    }

                    decrypted = this.EncodingProvider.GetString(ms.ToArray());
                } catch (Exception) {
                } finally {
                    if (ms != null) {
                        ms.Close();
                        ms.Dispose();
                    }
                    if (ct != null) {
                        ct.Dispose();
                    }
                }
            }

            return decrypted;
        }
    }
}
