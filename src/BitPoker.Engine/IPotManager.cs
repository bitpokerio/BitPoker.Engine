using System;
using System.Collections.Generic;

namespace BitPoker.Engine
{
    public interface IPotManager
    {
        IList<Pot> Pots { get; }
    }
}
