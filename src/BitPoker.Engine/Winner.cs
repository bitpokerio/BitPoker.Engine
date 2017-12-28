using System;

namespace BitPoker.Engine
{
    /// <summary>
    /// Simple data class representing the winner of a Pot.
    /// 
    /// Author: Wesley Tansey
    /// </summary>
    public class Winner
    {
        public readonly UInt64 Amount;
        public readonly string Player;
        public readonly string Pot;

        public Winner(string player, string pot, UInt64 amount)
        {
            Player = player;
            Pot = pot;
            Amount = amount;            
        }
    }
}
