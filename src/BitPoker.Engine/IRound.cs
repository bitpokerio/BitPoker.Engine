using System;

namespace BitPoker.Engine
{
    public interface IRound
    {
        void Start();

        int Hero { get; }

        Boolean RoundOver { get; }
    }
}