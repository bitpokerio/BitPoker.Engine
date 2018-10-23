using System;
using System.Collections.Generic;
using System.Text;
using BitPoker.Engine;

namespace ConsoleApp
{
    /// <summary>
    /// Plays of a hand of poker between a list of players. Designed for fast local use by AI agents.
    /// 
    /// Author: Wesley Tansey
    /// </summary>
    public class HandEngine 
    {
        Seat[] _seats;

        BetManager _betManager;
        PotManager _potManager;
        CircularList<int> _playerIndices;


        int _buttonIdx;
        int _utgIdx;
        int _bbIdx;
        
        private string tableName;

        private UInt64[] hc;


        private BettingStructure bs;


        private List<Winner> winners;
        
        private UInt64[] startingChips;
        private int maxPlayersPerTable;
        private uint dealtCards;

        public int CurrentBetLevel { get; private set; }
        public Round CurrentRound { get; private set; }

        ICollection<BitPoker.Engine.Action> curRoundActions;

        public IList<BitPoker.Engine.Action> CurrentActions
        {
            get
            {
                switch (this.CurrentRound)
                {
                    case Round.Predeal: return PredealActions;
                    case Round.Preflop: return PreflopActions;
                    case Round.Flop: return FlopActions;
                    case Round.Turn: return TurnActions;
                    case Round.River: return RiverActions;
                    default: break;
                }
                return null;
            }
        }

        public uint Board
        {
            get { return Flop | Turn | River; }
        }

        public UInt64[] AllBlinds { get; set; }

        public uint DealtCards { get; set; }

        public uint Flop { get; private set; }
        public uint Turn { get; private set; } 
        public uint River { get; private set; }

        
        public IList<BitPoker.Engine.Action> PredealActions { get; private set;}
        
        public IList<BitPoker.Engine.Action> PreflopActions { get; private set; }
        
        public IList<BitPoker.Engine.Action> FlopActions { get; private set;}

        public IList<BitPoker.Engine.Action> TurnActions { get; private set; }

        public IList<BitPoker.Engine.Action> RiverActions { get; private set;}

        public IList<Winner> Winners { get; private set; }

        public UInt64[] StartingChips
        {
            get { return startingChips; }
            set { startingChips = value; }
        }

        public int MaxPlayersPerTable { get; set; }

        public string Site { get; set; }

        public string TableName { get; set; }

        public UInt64 Stakes { get; set; }

        public UInt64 Pot { get; set; }

        public BettingStructure BettingStructure
        {
            get { return bs; }
            set { bs = value; }
        }
	
        public UInt64[] HoleCards
        {
            get { return hc; }
            set { hc = value; }
        }

        //Betting states
        public UInt64 Ante { get; private set; }

        public UInt64 SmallBlind { get; private set; }

        public UInt64 BigBlind { get; private set;}

        public UInt64 HandNumber { get; set; }

        //TODO: Button index
        public uint Button { get; private set ;}
                   
        
        public bool[] Folded { get; set; }

        public bool[] AllIn { get; set; }

        public bool ShowDown { get; set; }

        /// <summary>
        /// The index of the player who has to act currently
        /// </summary>
        public int Hero { get; set; }


        /// <summary>
        /// MAYBE ROUND OVER
        /// </summary>
        /// <value><c>true</c> if hand over; otherwise, <c>false</c>.</value>
        public Boolean HandOver 
        { 
            get
            {
                return this.ShowDown;
            }
        }

        public IEnumerable<BitPoker.Engine.Action> GetValidActions()
        {
            List<BitPoker.Engine.Action> validActions = new List<BitPoker.Engine.Action>();

            //TODO:  CHECK ROUND OVER OR HAND OVER
            if (!HandOver)
            {
                int pIdx = 0; //GetFirstToAct();
                var name = _seats[pIdx].Name;

                BitPoker.Engine.Action fold = new BitPoker.Engine.Action(name, BitPoker.Engine.Action.ActionTypes.Fold);
                fold = _betManager.GetValidatedAction(fold);
                validActions.Add(fold);//may be check or fold

                if (fold.ActionType == BitPoker.Engine.Action.ActionTypes.Fold)
                {
                    BitPoker.Engine.Action call = new BitPoker.Engine.Action(name, BitPoker.Engine.Action.ActionTypes.Call);
                    call = _betManager.GetValidatedAction(call);
                    validActions.Add(call);
                }

                BitPoker.Engine.Action minRaise = new BitPoker.Engine.Action(name, BitPoker.Engine.Action.ActionTypes.Raise, 0);
                minRaise = _betManager.GetValidatedAction(minRaise);

                if (minRaise.ActionType == BitPoker.Engine.Action.ActionTypes.Bet || minRaise.ActionType == BitPoker.Engine.Action.ActionTypes.Raise)
                {
                    validActions.Add(minRaise);

                    // In no-limit and pot-limit, we return the valid raises as a pair of
                    // (min, max) bets.
                    if (!minRaise.AllIn && this.BettingStructure != BettingStructure.Limit)
                    {
                        BitPoker.Engine.Action maxRaise = new BitPoker.Engine.Action(name, BitPoker.Engine.Action.ActionTypes.Raise, _seats[pIdx].Chips);
                        maxRaise = _betManager.GetValidatedAction(maxRaise);
                        if (maxRaise.Amount > minRaise.Amount)
                            validActions.Add(maxRaise);
                    }
                }
            }

            return validActions;
        }

        public HandEngine()
        {
        }

        public HandEngine(Seat[] players, ulong handNumber, uint button, UInt64[] blinds)
        {
            _seats = players;

            this.HoleCards = new ulong[_seats.Length];
            this.DealtCards = 0;
            this.Flop = 0;
            this.Turn = 0;
            this.River = 0;

            //Create a new map from player names to player chips for the BetManager
            Dictionary<string, UInt64> namesToChips = new Dictionary<string, UInt64>();

            //Create a new list of players for the PlayerManager
            _playerIndices = new CircularList<int>();
            _playerIndices.Loop = true;

            for (int i = 0; i < _seats.Length; i++)
            {
                namesToChips[_seats[i].Name] = _seats[i].Chips;
                if (_seats[i].SeatNumber == this.Button) //Button index
                {
                    _buttonIdx = i;
                    _utgIdx = (i + 1) % _seats.Length;
                }
            }

            for (int i = (_buttonIdx + 1) % _seats.Length; _playerIndices.Count < _seats.Length;)
            {
                _playerIndices.Add(i);
                i = (i + 1) % _seats.Length;
            }

            _betManager = new BetManager(namesToChips, this.BettingStructure, blinds, 0);
            _potManager = new PotManager(_seats);
        }

        /// <summary>
        /// Plays a hand from the start. Note that this method will <b>not</b> resume a game from a saved hand _history.
        /// </summary>
        [Obsolete]
        public void PlayHand()
        {
            if (_betManager.In > 1)
            {
                GetBlinds();
                DealHoleCards();
            }
            
            this.CurrentRound = Round.Preflop;

            if (_betManager.CanStillBet > 1)
            {
                //dont need to inject actions, we know the state
                GetBets();
                //GetBets(this.PreflopActions);
            }
            if (_betManager.In <= 1)
            {
                payWinners();
                return;
            }

            DealFlop();
            this.CurrentRound = Round.Flop;

            if (_betManager.CanStillBet > 1)
            {
                //dont need to inject actions, we know the state
                GetBets();
                //GetBets(this.FlopActions);
            }
            if (_betManager.In <= 1)
            {
                payWinners();
                return;
            }
            
            DealTurn();
            this.CurrentRound = Round.Turn;

            if (_betManager.CanStillBet > 1)
            {
                //dont need to inject actions, we know the state
                GetBets();
                //GetBets(this.TurnActions);
            }
            
            if (_betManager.In <= 1)
            {
                payWinners();
                return;
            }

            DealRiver();
            this.CurrentRound = Round.River;

            if (_betManager.CanStillBet > 1)
            {
                //dont need to inject actions, we know the state
                GetBets();
                //GetBets(this.RiverActions);
            }
            if (_betManager.In <= 1)
            {
                payWinners();
                return;
            }

            payWinners();
            this.ShowDown = true;
            this.CurrentRound = Round.Over;
        }

        /// <summary>
        /// Gets the bets from all the players still in the hand.
        /// </summary>
        public void GetBets()
        {
            bool roundOver = false;
            
            int pIdx = GetFirstToAct(this.CurrentRound == Round.Preflop);
            
            //keep getting bets until the round is over
            while (!roundOver)
            {
                this.CurrentBetLevel = _betManager.BetLevel;
                this.Pot = _potManager.Total;
                this.Hero = pIdx;
                
                //get the next player's action
                BitPoker.Engine.Action.ActionTypes actionType; 
                UInt64 amount;

                _seats[pIdx].Brain.GetAction(_history, out actionType, out amount);

                AddAction(pIdx, new BitPoker.Engine.Action(_seats[pIdx].Name, actionType, amount));

                roundOver = _betManager.RoundOver;

                if(!roundOver)
                {
                    pIdx = _playerIndices.Next;
                }
            }
        }

        //TODO: CHANGE TO NEXT TO ACT
        /// <summary>
        /// Gets the first to act.
        /// </summary>
        /// <returns>The first to act.</returns>
        /// <param name="preflop">If set to <c>true</c> preflop.</param>
        public int GetFirstToAct(bool preflop)
        {
            int desired = ((preflop ? _bbIdx : _buttonIdx) + 1) % _seats.Length;
            while (!_playerIndices.Contains(desired)) { desired = (desired + 1) % _seats.Length; }
            while(_playerIndices.Next != desired){}

            return desired;
        }

        public void AddAction(int pIdx, BitPoker.Engine.Action action)
        {
            action = _betManager.GetValidatedAction(action);
            _betManager.Commit(action);

            this.CurrentActions.Add(action); //CHECK THIS

            if (action.Amount > 0)
            {
                _seats[pIdx].Chips -= action.Amount;
            }

            //update the pots
            _potManager.AddAction(pIdx, action);

            if (action.ActionType == BitPoker.Engine.Action.ActionTypes.None)
            {
                throw new Exception("Must have an action");
            }

            //if the player either folded or went all-in, they can no longer
            //bet so remove them from the player pool
            if (action.ActionType == BitPoker.Engine.Action.ActionTypes.Fold)
            {
                _playerIndices.Remove(pIdx);
                this.Folded[pIdx] = true;
            }
            else if (action.AllIn)
            {
                _playerIndices.Remove(pIdx);
                this.AllIn[pIdx] = true;
            }
        }


        /// <summary>
        /// Forces players to post blinds before the hand can start.
        /// </summary>
        public void GetBlinds()
        {
            if (this.Ante > 0)
            {
                for (int i = _utgIdx, count = 0; count < _seats.Length; i = (i + 1) % _seats.Length, count++)
                {
                    //AddAction(i, new BitPoker.Engine.Action(_seats[i].Name, BitPoker.Engine.Action.ActionTypes.PostAnte, this.Ante), this.PredealActions);
                    AddAction(i, new BitPoker.Engine.Action(_seats[i].Name, BitPoker.Engine.Action.ActionTypes.PostAnte, this.Ante));
                }
            }

            // If there is no small blind, the big blind is the utg player, otherwise they're utg+1
            _bbIdx = _playerIndices.Next;
            if (this.SmallBlind > 0)
            {
                // If there was an ante and the small blind was put all-in, they can't post the small blind
                if (_playerIndices.Contains(_utgIdx))
                {
                    AddAction(_bbIdx, new BitPoker.Engine.Action(_seats[_bbIdx].Name, BitPoker.Engine.Action.ActionTypes.PostSmallBlind, this.SmallBlind));
                }

                _bbIdx = _playerIndices.Next;
            }
            
            if (this.BigBlind > 0)
            {
                if (_playerIndices.Contains(_bbIdx))
                {
                    AddAction(_bbIdx, new BitPoker.Engine.Action(_seats[_bbIdx].Name, BitPoker.Engine.Action.ActionTypes.PostBigBlind, this.BigBlind);
                    //AddAction(_bbIdx, new BitPoker.Engine.Action(_seats[_bbIdx].Name, BitPoker.Engine.Action.ActionTypes.PostBigBlind, this.BigBlind), this.PredealActions);
                }
            }
        }

        /// <summary>
        /// Deals out all of the players' hole cards.
        /// </summary>
        public void DealHoleCards()
        {
            for (int i = 0; i < _seats.Length; i++)
            {
                //this.HoleCards[i] = _cache != null ? _cache.HoleCards[i] : Hand.RandomHand(_history.DealtCards, 2);
                this.HoleCards[i] = Hand.RandomHand(DealtCards, 2);
                this.DealtCards = this.DealtCards | this..HoleCards[i];
            }
        }

        public void DealFlop()
        {
            this.Flop = Hand.RandomHand(this.DealtCards, 3);
            this.DealtCards = this.DealtCards | this.Flop;
        }

        public void DealTurn()
        {
            this.Turn = Hand.RandomHand(this.DealtCards, 1);
            this.DealtCards = this.DealtCards | this.Turn;
        }

        public void DealRiver()
        {
            this.River = Hand.RandomHand(this.DealtCards, 1);
            this.DealtCards = this.DealtCards | this.River;
        }

        private void payWinners()
        {
            uint[] strengths = new uint[_seats.Length];
            for (int i = 0; i < strengths.Length; i++)
            {
                if(!this.Folded[i])
                {
                    //TODO: ADD A HAND EVALUATE BACK IN
                    //strengths[i] = Hand.Evaluate(this.HoleCards[i] | this.Board, 7);
                }
            }
            
            this.Winners = _potManager.GetWinners(strengths);;
        }
    }
}