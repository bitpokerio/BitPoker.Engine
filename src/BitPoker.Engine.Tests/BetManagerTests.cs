using System;
using System.Collections.Generic;
using BitPoker.Engine;
using Xunit;
using FastPokerAction = BitPoker.Engine.Action;

namespace BitPoker.Engine.Tests
{
    public class BetManagerTests
    {
        private BetManager betMan;
        private Dictionary<string, UInt64> namesToChips;
        
        [Fact]
        public void TestBlinds()
        {
            namesToChips = new Dictionary<string, UInt64>();
            namesToChips["Player0"] = 200;
            namesToChips["Player1"] = 200;
            namesToChips["Player2"] = 200;
            namesToChips["Player3"] = 200;
            namesToChips["Player4"] = 200;
        
            UInt64[] blinds = new UInt64[]{1,2};
            betMan = new BetManager(namesToChips, BettingStructure.NoLimit, blinds, 0);

            FastPokerAction[] actions = new FastPokerAction[] {
                new FastPokerAction("Player3", FastPokerAction.ActionTypes.PostSmallBlind, 25),
                new FastPokerAction("Player4", FastPokerAction.ActionTypes.PostBigBlind, 50),
                new FastPokerAction("Player0", FastPokerAction.ActionTypes.Raise, 1),
            };

            FastPokerAction action = betMan.GetValidatedAction(actions[0]);
            Assert.True(1 == action.Amount);
            Assert.Equal(FastPokerAction.ActionTypes.PostSmallBlind, action.ActionType);
            
            betMan.Commit(action);
            Assert.False(betMan.RoundOver);

            action = betMan.GetValidatedAction(actions[1]);
            Assert.True(2 == action.Amount);
            Assert.Equal(FastPokerAction.ActionTypes.PostBigBlind, action.ActionType);

            betMan.Commit(action);
            Assert.False(betMan.RoundOver);

            action = betMan.GetValidatedAction(actions[2]);
            Assert.True(4 == action.Amount);
            Assert.Equal(Engine.Action.ActionTypes.Raise, action.ActionType);

            betMan.Commit(action);
            Assert.False(betMan.RoundOver);
        }
    }
}
