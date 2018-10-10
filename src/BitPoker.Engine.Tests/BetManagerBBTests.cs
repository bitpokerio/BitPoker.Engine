using System;
using System.Collections.Generic;
using BitPoker.Engine;
using Xunit;
using FastPokerAction = BitPoker.Engine.Action;

namespace BitPoker.Engine.Tests
{
    public class BetManagerBBTests
    {
        private Dictionary<string, UInt64> namesToChips;

        public BetManagerBBTests()
        {
            namesToChips["Player0"] = 200;
            namesToChips["Player1"] = 200;
        }

        [Fact]
        public void Should_Add_Big_Blind()
        {
            // namesToChips["Player0"] = 200;
            // namesToChips["Player1"] = 200;
            // namesToChips["Player2"] = 200;
            // namesToChips["Player3"] = 200;
            // namesToChips["Player4"] = 200;
        
            UInt64[] blinds = new UInt64[]{1,2};
            BetManager betMan = new BetManager(namesToChips, BettingStructure.NoLimit, blinds, 0);

            FastPokerAction action = betMan.GetValidatedAction(new FastPokerAction("Player0", FastPokerAction.ActionTypes.PostSmallBlind, 1));
            Assert.True(1 == action.Amount);
            Assert.Equal(FastPokerAction.ActionTypes.PostSmallBlind, action.ActionType);

            Assert.False(betMan.RoundOver);

            betMan.Commit(action);
            Assert.False(betMan.RoundOver);

            action = betMan.GetValidatedAction(new FastPokerAction("Player1", FastPokerAction.ActionTypes.PostSmallBlind, 1));
            Assert.True(2 == action.Amount);
            Assert.Equal(FastPokerAction.ActionTypes.PostBigBlind, action.ActionType);
        }
    }
}
