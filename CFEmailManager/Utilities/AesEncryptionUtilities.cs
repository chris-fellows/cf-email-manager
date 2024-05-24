using System.Security.Cryptography;
using System.Text;

namespace CFEmailManager.Utilities
{
    /// <summary>
    /// AES encryption utilities
    /// </summary>
    internal class AesEncryptionUtilities
    { 
      /// <summary>
      /// Encrypts string
      /// </summary>
      /// <param name="plainText"></param>
      /// <param name="key">Key</param>
      /// <param name="iv">Initialization vector</param>
      /// <returns></returns>
        public static byte[] Encrypt(string plainText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] encryptedBytes;
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    }
                    encryptedBytes = memoryStream.ToArray();
                }
                return encryptedBytes;
            }
        }

        /// <summary>
        /// Decrypts bytes to string
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key">Key</param>
        /// <param name="iv">Initialization vector</param>
        /// <returns></returns>
        public static string Decrypt(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] decryptedBytes;
                using (var streamCipher = new System.IO.MemoryStream(cipherText))
                {
                    using (var cryptoStream = new CryptoStream(streamCipher, decryptor, CryptoStreamMode.Read))
                    {
                        using (var streamPlain = new System.IO.MemoryStream())
                        {
                            cryptoStream.CopyTo(streamPlain);
                            decryptedBytes = streamPlain.ToArray();
                        }
                    }
                }
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        /// <summary>
        /// Creates random key or IV of specified byte length
        /// </summary>
        /// <param name="bytesLength"></param>
        /// <returns>Key or IV</returns>
        public static byte[] CreateRandomKeyOrIV(int bytesLength)
        {
            var bytes = new byte[bytesLength];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }

        //public static void Test()
        //{
        //    string plaintext = "Hello, World!";
        //    Console.WriteLine(plaintext);
        //    // Generate a random key and IV
        //    byte[] key = new byte[32]; // 256-bit key
        //    byte[] iv = new byte[16]; // 128-bit IV
        //    using (var rng = new RNGCryptoServiceProvider())
        //    {
        //        rng.GetBytes(key);
        //        rng.GetBytes(iv);
        //    }

        //    // Encrypt
        //    byte[] ciphertext = Encrypt(plaintext, key, iv);
        //    string encryptedText = Convert.ToBase64String(ciphertext);
        //    Console.WriteLine("Encrypted Text: " + encryptedText);
        //    // Decrypt
        //    byte[] bytes = Convert.FromBase64String(encryptedText);
        //    string decryptedText = Decrypt(bytes, key, iv);
        //    Console.WriteLine("Decrypted Text: " + decryptedText);

        //    int xxxx = 1000;
        //}
    }
}
