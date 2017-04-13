using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class SecuritySenderECD : ISecuritySender
    {
        private CngKey _privateKey;
        public byte[] PublicKey { get; }

        public SecuritySenderECD(CngKey privateKey, byte[] publicKey)
        {
            _privateKey = privateKey;
            this.PublicKey = publicKey;
        }

        public async Task<string> ReceiveMessage(string msg, byte[] senderPubKeyRaw)
        {
            var aes = new AesCryptoServiceProvider();
            var encryptedData = Convert.FromBase64String(msg);
            int nBytes = 16;//aes.BlockSize;
            byte[] decryptedData;
            byte[] iv = new byte[nBytes];
            Array.Copy(encryptedData, iv, nBytes);
            using (var algorithm = new ECDiffieHellmanCng(_privateKey))
            using (var senderPubKey = CngKey.Import(senderPubKeyRaw, CngKeyBlobFormat.EccPublicBlob))
            {
                var symKey = algorithm.DeriveKeyMaterial(senderPubKey);
                aes.Key = symKey;
                aes.IV = iv;
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        await cryptoStream.WriteAsync(encryptedData, nBytes, encryptedData.Length - nBytes);
                    }
                    decryptedData = memoryStream.ToArray();
                }
                aes.Clear();
            }
            //string -> utf8 byte[] -> base64  string -> base64 byte[] -> utf8 string 
            return Encoding.UTF8.GetString(decryptedData);
        }

        public async Task<string> SendMessage(string msg, byte[] receiverPubKeyRaw)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            byte[] encryptedData;

            using (var algorithm = new ECDiffieHellmanCng(_privateKey))
            using (var receiverPubKey = CngKey.Import(receiverPubKeyRaw, CngKeyBlobFormat.EccPublicBlob))
            {
                //generate symmetric key
                var symKey = algorithm.DeriveKeyMaterial(receiverPubKey);
                using (var aes = new AesCryptoServiceProvider())
                {
                    aes.Key = symKey;
                    aes.GenerateIV();
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            await memoryStream.WriteAsync(aes.IV, 0, aes.IV.Length);
                            cryptoStream.Write(data, 0, data.Length);
                        }
                        encryptedData = memoryStream.ToArray();
                    }
                    aes.Clear();
                }
            }
            return Convert.ToBase64String(encryptedData);
        }
    }
}