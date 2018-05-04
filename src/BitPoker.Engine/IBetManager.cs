using System;
namespace BitPoker.Engine
{
    public interface IBetManager
    {
        Boolean RoundOver { get; }
        
        int BetLevel { get; set; }
    }
}
