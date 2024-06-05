using System;
using System.Security.Cryptography;
using System.IO;

namespace CFEmailManager.Utilities
{
    /// <summary>
    /// Encrypts using Data Protection API
    /// </summary>
    public class DataProtectionUtilities
    {
        /// <summary>
        /// Encrypts data for specific scope
        /// </summary>
        /// <param name="data"></param>
        /// <param name="entropy"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] data, byte[] entropy, DataProtectionScope scope)
        {
            using (var stream = new MemoryStream())
            {
                int bytesWritten = EncryptDataToStream(data, entropy, scope, stream);
                stream.Position = 0;

                var encrypted = new byte[bytesWritten];
                stream.Read(encrypted, 0, bytesWritten);
                return encrypted;                
            }
        }

        /// <summary>
        /// Decrypts data for specific scope
        /// </summary>
        /// <param name="data"></param>
        /// <param name="entropy"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] data, byte[] entropy, DataProtectionScope scope)
        {
            using (var stream = new MemoryStream())
            {                
                stream.Write(data, 0, data.Length);
                stream.Position = 0;

                return DecryptDataFromStream(entropy, scope, stream, data.Length);                
            }
        }

        //public static void Run()
        //{
        //    try
        //    {
        //        ///////////////////////////////
        //        //
        //        // Memory Encryption - ProtectedMemory
        //        //
        //        ///////////////////////////////

        //        // Create the original data to be encrypted (The data length should be a multiple of 16).
        //        byte[] toEncrypt = UnicodeEncoding.ASCII.GetBytes("ThisIsSomeData16");

        //        Console.WriteLine($"Original data: {UnicodeEncoding.ASCII.GetString(toEncrypt)}");
        //        Console.WriteLine("Encrypting...");

        //        // Encrypt the data in memory.
        //        EncryptInMemoryData(toEncrypt, MemoryProtectionScope.SameLogon);

        //        Console.WriteLine($"Encrypted data: {UnicodeEncoding.ASCII.GetString(toEncrypt)}");
        //        Console.WriteLine("Decrypting...");

        //        // Decrypt the data in memory.
        //        DecryptInMemoryData(toEncrypt, MemoryProtectionScope.SameLogon);

        //        Console.WriteLine($"Decrypted data: {UnicodeEncoding.ASCII.GetString(toEncrypt)}");

        //        ///////////////////////////////
        //        //
        //        // Data Encryption - ProtectedData
        //        //
        //        ///////////////////////////////

        //        // Create the original data to be encrypted
        //        toEncrypt = UnicodeEncoding.ASCII.GetBytes("This is some data of any length.");

        //        // Create a file.
        //        FileStream fStream = new FileStream("C:\\Temp\\DataProtectionTest\\Data.dat", FileMode.OpenOrCreate);

        //        // Create some random entropy.
        //        byte[] entropy = CreateRandomEntropy();

        //        Console.WriteLine();
        //        Console.WriteLine($"Original data: {UnicodeEncoding.ASCII.GetString(toEncrypt)}");
        //        Console.WriteLine("Encrypting and writing to disk...");

        //        // Encrypt a copy of the data to the stream.
        //        int bytesWritten = EncryptDataToStream(toEncrypt, entropy, DataProtectionScope.CurrentUser, fStream);

        //        fStream.Close();

        //        Console.WriteLine("Reading data from disk and decrypting...");

        //        // Open the file.
        //        fStream = new FileStream("C:\\Temp\\DataProtectionTest\\Data.dat", FileMode.Open);

        //        // Read from the stream and decrypt the data.
        //        byte[] decryptData = DecryptDataFromStream(entropy, DataProtectionScope.CurrentUser, fStream, bytesWritten);

        //        fStream.Close();

        //        Console.WriteLine($"Decrypted data: {UnicodeEncoding.ASCII.GetString(decryptData)}");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine($"ERROR: {e.Message}");
        //    }
        //}

        //public static void EncryptInMemoryData(byte[] Buffer, MemoryProtectionScope Scope)
        //{
        //    if (Buffer == null)
        //        throw new ArgumentNullException(nameof(Buffer));
        //    if (Buffer.Length <= 0)
        //        throw new ArgumentException("The buffer length was 0.", nameof(Buffer));

        //    // Encrypt the data in memory. The result is stored in the same array as the original data.
        //    ProtectedMemory.Protect(Buffer, Scope);
        //}

        //public static void DecryptInMemoryData(byte[] Buffer, MemoryProtectionScope Scope)
        //{
        //    if (Buffer == null)
        //        throw new ArgumentNullException(nameof(Buffer));
        //    if (Buffer.Length <= 0)
        //        throw new ArgumentException("The buffer length was 0.", nameof(Buffer));

        //    // Decrypt the data in memory. The result is stored in the same array as the original data.
        //    ProtectedMemory.Unprotect(Buffer, Scope);
        //}

        public static byte[] CreateRandomEntropy()
        {
            // Create a byte array to hold the random value.
            byte[] entropy = new byte[16];

            // Create a new instance of the RNGCryptoServiceProvider.
            // Fill the array with a random value.
            new RNGCryptoServiceProvider().GetBytes(entropy);

            // Return the array.
            return entropy;
        }

        private static int EncryptDataToStream(byte[] data, byte[] entropy, DataProtectionScope scope, Stream stream)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length <= 0)
                throw new ArgumentException("The buffer length was 0.", nameof(data));
            if (entropy == null)
                throw new ArgumentNullException(nameof(entropy));
            if (entropy.Length <= 0)
                throw new ArgumentException("The entropy length was 0.", nameof(entropy));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));            

            // Encrypt the data and store the result in a new byte array. The original data remains unchanged.
            byte[] encryptedData = ProtectedData.Protect(data, entropy, scope);

            // Write the encrypted data to a stream.
            int length = 0;
            if (stream.CanWrite && encryptedData != null)
            {
                stream.Write(encryptedData, 0, encryptedData.Length);

                length = encryptedData.Length;
            }

            // Return the length that was written to the stream.
            return length;
        }

        private static byte[] DecryptDataFromStream(byte[] entropy, DataProtectionScope scope, Stream stream, int length)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (length <= 0)
                throw new ArgumentException("The given length was 0.", nameof(length));
            if (entropy == null)
                throw new ArgumentNullException(nameof(entropy));
            if (entropy.Length <= 0)
                throw new ArgumentException("The entropy length was 0.", nameof(entropy));

            byte[] inBuffer = new byte[length];
            byte[] outBuffer;

            // Read the encrypted data from a stream.
            if (stream.CanRead)
            {
                stream.Read(inBuffer, 0, length);
                outBuffer = ProtectedData.Unprotect(inBuffer, entropy, scope);
            }
            else
            {
                throw new IOException("Could not read the stream.");
            }

            // Return the decrypted data
            return outBuffer;
        }
    }
}
