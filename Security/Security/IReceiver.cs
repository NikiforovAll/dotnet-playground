using System.Threading.Tasks;

namespace Security
{
    interface IReceiver
    {
        Task<string> ReceiveMessage(string msg, byte[] senderPubKeyRaw);
    }
}