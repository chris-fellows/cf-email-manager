using MimeKit;
using CFEmailManager.Model;
using System.Text;
using System;
using CFUtilities.Encryption;

namespace CFEmailManager.Utilities
{
    internal class InternalUtilities
    {      
        /// <summary>
        /// Generates a unique key for the email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetEmailKey(EmailObject email)
        {
            return string.Format("{0}|{1}|{2}", email.From.Address.ToLower(), email.ReceivedDate.ToString("yyyy-MM-dd HHmmss"), email.Subject.ToLower());
        }

        /// <summary>
        /// Generates a unique key for the email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetEmailKey(MimeMessage email)
        {
            return $"{email.MessageId}";
        }

        /// <summary>
        /// Encrypts setting using Data Protection API
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncryptSettingByDPToString(string value)
        {
            var entropy = Convert.FromBase64String(System.Configuration.ConfigurationSettings.AppSettings.Get("Random1").ToString());
          
            var data = UnicodeEncoding.Unicode.GetBytes(value);
            var encrypted = DataProtectionUtilities.Encrypt(data, entropy, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts setting to string using Data Protection API
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecryptSettingByDPToString(string value)
        {
            var entropy = Convert.FromBase64String(System.Configuration.ConfigurationSettings.AppSettings.Get("Random1").ToString());

            var encrypted = Convert.FromBase64String(value);
            var decrypted = DataProtectionUtilities.Decrypt(encrypted, entropy, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return UnicodeEncoding.Unicode.GetString(decrypted);
        }

        /// <summary>
        /// Decrypts setting to bytes using Data Protection API
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] DecryptSettingByDPToBytes(string value)
        {
            var entropy = Convert.FromBase64String(System.Configuration.ConfigurationSettings.AppSettings.Get("Random1").ToString());

            var encrypted = Convert.FromBase64String(value);
            var decrypted = DataProtectionUtilities.Decrypt(encrypted, entropy, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return decrypted;
        }

        /// <summary>
        /// Encrypts setting to string using AES encryption. Keys are accessed using Data Protection API
        /// </summary>
        /// <param name="password"></param>
        public static string EncryptSettingToString(string password)
        {
            var key = InternalUtilities.DecryptSettingByDPToBytes(System.Configuration.ConfigurationSettings.AppSettings.Get("Random2").ToString());
            var iv = InternalUtilities.DecryptSettingByDPToBytes(System.Configuration.ConfigurationSettings.AppSettings.Get("Random3").ToString());
            
            var encrypted = Convert.ToBase64String(AESEncryption.Encrypt(Encoding.UTF8.GetBytes(password), key, iv));            
            return encrypted;
        }

        /// <summary>
        /// Decrypts setting to string using AES encryption. Keys are access using Data Protection API
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string DecryptSettingToString(string setting)
        {
            var key = InternalUtilities.DecryptSettingByDPToBytes(System.Configuration.ConfigurationSettings.AppSettings.Get("Random2").ToString());
            var iv = InternalUtilities.DecryptSettingByDPToBytes(System.Configuration.ConfigurationSettings.AppSettings.Get("Random3").ToString());

            var decrypted = Encoding.UTF8.GetString(AESEncryption.Decrypt(Convert.FromBase64String(setting), key, iv));
            return decrypted;
        }
    }
}

