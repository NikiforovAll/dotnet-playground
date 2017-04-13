using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Security
{
    partial class Program
    {
        static void Main(string[] args)
        {
            IList<ISecuritySender> endpoints = new List<ISecuritySender> { };

            Task.Run(async () =>
            {
                switch (args[0])
                {
                    case "1":
                        endpoints = new[]{
                          new SecuritySenderECD(
                              GenerateSecretsECD(out CngKey key1),
                              key1.Export(CngKeyBlobFormat.GenericPublicBlob)),
                          new SecuritySenderECD(
                              GenerateSecretsECD(out CngKey key2),
                              key2.Export(CngKeyBlobFormat.GenericPublicBlob))
                        };
                        break;
                    case "2":
                        endpoints = new[]{
                          new SecuritySenderRSA(
                              GenerateSecretsRSA(out CngKey key3),
                              key3.Export(CngKeyBlobFormat.GenericPublicBlob)),
                          new SecuritySenderRSA(
                              GenerateSecretsRSA(out CngKey key4),
                              key4.Export(CngKeyBlobFormat.GenericPublicBlob))
                        };
                        break;
                }
                var sender = endpoints[0] as ISender;
                var receiver = endpoints[1] as IReceiver;
                var secretMessage = "secret message";
                var encryptedMessage = await sender.SendMessage(secretMessage, endpoints[1].PublicKey);
                var decryptedMessage = await receiver.ReceiveMessage(encryptedMessage, endpoints[0].PublicKey);
                Console.WriteLine($"message:  {secretMessage}");
                Console.WriteLine($"decryptedMessage: {decryptedMessage}");
            }).GetAwaiter().GetResult();

        }

        public static CngKey GenerateSecretsECD(out CngKey key)
        {
            key = CngKey.Create(CngAlgorithm.ECDiffieHellmanP521);
            return key;
        }

        public static CngKey GenerateSecretsRSA(out CngKey key)
        {
            key = CngKey.Create(CngAlgorithm.Rsa);
            return key;
        }
    }
}
