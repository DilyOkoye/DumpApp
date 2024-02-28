using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Runtime.Caching;
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

        public static DataTable GetTop20DumpRecords()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionProc"].ToString();

            string cacheKey = "Top10DumpRecordsCacheKey";
            var cacheItem = MemoryCache.Default.Get(cacheKey) as DataTable;

            if (cacheItem != null)
            {
                // Return the cached data
                return cacheItem;
            }
            else
            {
                // Cache is empty or expired, fetch data from database
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 20 * FROM admDump order by DumpDate desc"; // Update this with your actual table name
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                // Cache the data with expiration time of 10 minutes
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                };
                MemoryCache.Default.Set(cacheKey, dt, policy);

                return dt;
            }
        }

        public static DataTable GetTop20LoadRecords()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionProc"].ToString();

            string cacheKey = "Top10LoadRecordsCacheKey";
            var cacheItem = MemoryCache.Default.Get(cacheKey) as DataTable;

            if (cacheItem != null)
            {
                // Return the cached data
                return cacheItem;
            }
            else
            {
                // Cache is empty or expired, fetch data from database
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 20 * FROM admLoad order by DumpDate desc"; // Update this with your actual table name
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                // Cache the data with expiration time of 10 minutes
                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                };
                MemoryCache.Default.Set(cacheKey, dt, policy);

                return dt;
            }
        }


        public static DataTable GetNotifications()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionProc"].ToString();

            string cacheKey = "DumpNotification";
            var cacheItem = MemoryCache.Default.Get(cacheKey) as DataTable;

            if (cacheItem != null)
            {
                // Return the cached data
                return cacheItem;
            }
            else
            {
                // Cache is empty or expired, fetch data from database
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlQuery = @"
    WITH StatusChanges AS (
        SELECT 
            *,
            LAG(status) OVER (ORDER BY DumpDate) AS previous_status
        FROM 
            admDump
        WHERE
            DumpDate >= DATEADD(day, -1, GETDATE()) -- Filter data for the last 24 hours
    )
    SELECT 
        *
    FROM 
        StatusChanges
    WHERE 
        status <> previous_status

    UNION

    SELECT 
        *,
        Status
    FROM 
        admLoad
    WHERE
        DumpDate >= DATEADD(day, -1, GETDATE()) -- Filter data for the last 24 hours
";


                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
                };
                MemoryCache.Default.Set(cacheKey, dt, policy);

                return dt;
            }
        }
    }
}
