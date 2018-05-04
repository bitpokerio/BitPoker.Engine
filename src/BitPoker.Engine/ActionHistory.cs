using System;
namespace BitPoker.Engine
{
    public class ActionHistory : Action
    {
        public String PreviousHash { get; }
        
        public ActionHistory()
        {
        }
        
        public Byte[] GetHash()
        {
            //Byte[] hash = Org.BouncyCastle.Crypto.
            return new byte[32];
        }
        
        public override string ToString()
        {
            return string.Format("[ActionHistory: PreviousHash={0}]", PreviousHash);
        }
    }
}
