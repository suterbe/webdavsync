
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace WebDavSyncGUI.Logic
{
    /// <summary>
    /// Klasse zum Ver/Entschlüsseln von Texten
    /// </summary>
    public  class Encryption
    {
        private const string ENCRYPTIONKEY = "ZrOORX411JqD4a46vOvrIIbnvyB5Xxkk";
        private const string VECTOR = "AJIhPpKhiqt=";
        private  SymmetricAlgorithm _mcsp;


        /// <summary>
        /// Verschlüsselt einen Text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Verschlüsselter Text</returns>
        public  string encrypt(string text)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            Byte[] byt;
            string returnvalue = "";

            if (text == "") { return ""; }

            _mcsp = new TripleDESCryptoServiceProvider();
            _mcsp.Key = Convert.FromBase64String(ENCRYPTIONKEY);
            _mcsp.IV = Convert.FromBase64String(VECTOR);

            try
            {
                ct = _mcsp.CreateEncryptor(_mcsp.Key, _mcsp.IV);
                byt = Encoding.UTF8.GetBytes(text);
                ms = new MemoryStream();
                cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                cs.Close();
                cs.Dispose();
            }
            catch
            {
                return "";
            }
            returnvalue = Convert.ToBase64String(ms.ToArray());
            ms.Close();
            ms.Dispose();
            ct.Dispose();
            return returnvalue;
        }

        /// <summary>
        /// Entschlüsselt einen Verschlüsselten String
        /// </summary>
        /// <param name="value">Verschlüsselter String</param>
        /// <returns>Unverschlüsselter Text</returns>
        public  string decrypt(string value)
        {
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            Byte[] byt;
            string returnvalue = "";

            if (value == "") { return ""; }

            _mcsp = new TripleDESCryptoServiceProvider();
            _mcsp.Key = Convert.FromBase64String(ENCRYPTIONKEY);
            _mcsp.IV = Convert.FromBase64String(VECTOR);

            try
            {
                ct = _mcsp.CreateDecryptor(_mcsp.Key, _mcsp.IV);
                byt = Convert.FromBase64String(value);

                ms = new MemoryStream();
                cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                cs.Close();
            }
            catch
            {
                return "";
            }
            returnvalue = Encoding.UTF8.GetString(ms.ToArray());

            ms.Close();
            ms.Dispose();
            ct.Dispose();
            return returnvalue;
        }
    }
}

