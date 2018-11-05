using System;
using System.Collections.Generic;
using BitPoker.Engine;
using Xunit;
using FastPokerAction = BitPoker.Engine.Action;

namespace BitPoker.Engine.Tests
{
    public class HandEngineTests
    {
        private Dictionary<string, UInt64> namesToChips;

        public HandEngineTests()
        {
            
        }

        [Fact]
        public void Should_Get()
        {
            BitPoker.Engine.HandEngine engine = new HandEngine();
        }
    }
}
