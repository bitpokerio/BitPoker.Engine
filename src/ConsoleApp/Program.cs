using System;

namespace ConsoleApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            
            Seat[] players = new Seat[5];

            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new Seat();
                players[i].Chips = 200.0;
                players[i].Name = "Test " + i;
                players[i].SeatNumber = i + 1;
            }

            Double[] blinds = new double[] { 1, 2 };



            HandEngine engine = new HandEngine(players, 0, 0, blinds);
            //engine.AddAction()


            while(!engine.HandOver)
            {
                int firstToAct = engine.GetFirstToAct(true);
                Console.WriteLine("Please act " + firstToAct);
                //Console.ReadLine();

                var action = new holdem_engine.Action("test1", holdem_engine.Action.ActionTypes.PostSmallBlind, 1);

                //is it valid
                var valid = engine.GetValidActions();

                engine.AddAction(firstToAct, action);
            }





            //holdem_engine.Action action = new holdem_engine.Action();


            ////
            //holdem_engine.Action[] actions0 = new holdem_engine.Action[] {
            //    new holdem_engine.Action("Seq0", holdem_engine.Action.ActionTypes.Raise, 4)
            //};

            //seqPlayers[0].Brain = new CryptoPlayer(actions0);

            //seqPlayers[1].Brain = new CryptoPlayer();
            //seqPlayers[2].Brain = new CryptoPlayer();
            //seqPlayers[3].Brain = new CryptoPlayer();


            ////seq2 is on _buttonIdx (seat 3), seq3 is small blind ($1), seq4 is big blind ($2), hand number is 42
            //HandHistory results = new TournamentHandHistory(seqPlayers, 42, 3, blinds, 0, BettingStructure.NoLimit);
            //engine.PlayHand(results);



            Console.Write("All in");
        }
    }
}