using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Security
{
    class SecuritySenderRSA : ISecuritySender
    {
        private CngKey _privateKey;
        public byte[] PublicKey { get; }
        public SecuritySenderRSA(CngKey privateKey, byte[] publicKey)
        {
            _privateKey = privateKey;
            this.PublicKey = publicKey;
        }

        public async Task<string> ReceiveMessage(string msg, byte[] senderPubKeyRaw)
        {
            CngKey senderPubKey = CngKey.Import(senderPubKeyRaw, CngKeyBlobFormat.GenericPublicBlob);
            bool isValidSignature = false;
            bool isUnchangedMessage = false;
            var tokens = msg.Split('.');
            using (var signingAlgorithm = new RSACng(senderPubKey))
            {
                isValidSignature = signingAlgorithm.VerifyHash(Convert.FromBase64String(tokens[0]), Convert.FromBase64String(tokens[1]), HashAlgorithmName.SHA384, RSASignaturePadding.Pss);
            }
            if (!isValidSignature)
            {
                throw new Exception("signature is no valid");
            }
            using (var hashAlgorithm = SHA384.Create())
            {
                isUnchangedMessage = Convert.FromBase64String(tokens[0]).SequenceEqual(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(tokens[2])));
            }
            if (!isUnchangedMessage)
            {
                throw new Exception("message is changed");
            }
            return await Task.Run<string>(() => tokens[2]);
        }
        /// <summary>
        /// returns hash, signature, and initial message in format {h}+{s}+{m}
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="receiverPubKeyRaw"></param>
        /// <returns></returns>
        public async Task<string> SendMessage(string msg, byte[] receiverPubKeyRaw)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            byte[] hash, signed;
            using (var hashAlgorithm = SHA384.Create())
            {
                hash = hashAlgorithm.ComputeHash(data);
            }
            using (var signingAlgorithm = new RSACng(_privateKey))
            {
                signed = signingAlgorithm.SignHash(hash, HashAlgorithmName.SHA384, RSASignaturePadding.Pss);
            }
            var result = $"{Convert.ToBase64String(hash)}.{Convert.ToBase64String(signed)}.{msg}";
            return await Task.Run<String>(() => result);
        }
    }
}
