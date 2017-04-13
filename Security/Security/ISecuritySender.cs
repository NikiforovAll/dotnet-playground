namespace Security
{
    interface ISecuritySender : ISender, IReceiver
    {
        byte[] PublicKey { get; }
    }
}
