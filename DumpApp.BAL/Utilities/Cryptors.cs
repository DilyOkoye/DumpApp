using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DumpApp.BAL.Utilities
{
    public class Cryptors
    {
        public static string EncryptLogin(string strToEncrpyt, string key)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = key;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strToEncrpyt));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB;
                byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrpyt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "wrong input. " + ex.Message;
            }
        }
        
        public static string Encrypt(string strToEncrypt, string key)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key)); // Hash the key instead of the plaintext
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; // Note: ECB mode is generally not recommended due to security weaknesses
                byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
                return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }
            catch (Exception ex)
            {
                return "Error in encryption: " + ex.Message;
            }
        }

        public static string Decrypt(string strEncrypted, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strKey)); // Use the key for hashing
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; // Note: Same note on ECB mode
                byteBuff = Convert.FromBase64String(strEncrypted);
                string strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Error in decryption: " + ex.Message;
            }
        }
    }
}
