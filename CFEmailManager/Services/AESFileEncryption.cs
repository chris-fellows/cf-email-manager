using CFEmailManager.Interfaces;
using CFEmailManager.Utilities;
using CFUtilities.Encryption;
using CFUtilities.Utilities;
using System.IO;

namespace CFEmailManager.Services
{
    /// <summary>
    /// Encrypts files using AES encrtpyion and deflate
    /// </summary>
    public class AESFileEncryption : IFileEncryption
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AESFileEncryption()
        {
            _key = InternalUtilities.DecryptSettingByDPToBytes(System.Configuration.ConfigurationSettings.AppSettings.Get("Random2").ToString());
            _iv = InternalUtilities.DecryptSettingByDPToBytes(System.Configuration.ConfigurationSettings.AppSettings.Get("Random3").ToString());
        }

        public string Name => "AES";

        public void WriteFile(string file, byte[] content)
        {
            var contentEncrypted = AESEncryption.Encrypt(CompressionUtilities.CompressWithDeflate(content), _key, _iv);
            File.WriteAllBytes(file, contentEncrypted);
        }
     
        public byte[] ReadFile(string file)
        {
            return CompressionUtilities.DecompressWithDeflate(AESEncryption.Decrypt(File.ReadAllBytes(file), _key, _iv));
        }
    }
}
