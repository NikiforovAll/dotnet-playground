using System.Threading.Tasks;

namespace Security
{
    interface ISender 
    {
        Task<string> SendMessage(string msg, byte[] receiverPubKeyRaw);
    }
}