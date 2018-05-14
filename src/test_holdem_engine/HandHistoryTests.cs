using NUnit.Framework;
using System;

namespace test_holdem_engine
{
    [TestFixture()]
    public class HandHistoryTests
    {
        public void Setup()
        {
            
        }

        [Test()]
        public void Should_Get_Pre_Deal()
        {
            holdem_engine.Seat[] players = new holdem_engine.Seat[5];

            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new holdem_engine.Seat();
                players[i].Chips = 200.0;
                players[i].Name = "Test " + i;
                players[i].SeatNumber = i + 1;
            }

            Double[] blinds = new double[] { 1, 2 };

            holdem_engine.HandHistory history = new holdem_engine.HandHistory(players, 0, 0, blinds, 0, holdem_engine.BettingStructure.NoLimit);

            var actual = history.CurrentRound;

            Assert.AreEqual(holdem_engine.Round.Predeal, actual);
        }

        [Test()]
        public void Should_Get_Pre_Flop()
        {
            holdem_engine.Seat[] players = new holdem_engine.Seat[5];

            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new holdem_engine.Seat();
                players[i].Chips = 200.0;
                players[i].Name = "Test " + i;
                players[i].SeatNumber = i + 1;
            }

            Double[] blinds = new double[] { 1, 2 };

            holdem_engine.HandHistory history = new holdem_engine.HandHistory(players, 0, 0, blinds, 0, holdem_engine.BettingStructure.NoLimit);

            var actual = history.CurrentActions;
            //var actual = history.CurrentRound;

            Assert.AreEqual(holdem_engine.Round.Predeal, actual);
        }
    }
}