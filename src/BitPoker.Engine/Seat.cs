using System;

namespace BitPoker.Engine
{
    public class Seat
    {
        /// <summary>
        /// The seat number that this player is at
        /// </summary>
        public int SeatNumber { get; set; }

        /// <summary>
        /// The amount of chips this player has.
        /// </summary>
        public UInt64 Chips { get; set; }
        
        public UInt64 Stack { get; set; }
        
        /// <summary>
        /// The name of this player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The brain of this player (i.e., the object that makes betting decisions).
        /// </summary>
        public IPlayer Brain { get; set; }
        
        public Boolean IsDealer { get; private set; }
        
        public Seat()
        {
        }

        public Seat(int seatNumber, string playerName, UInt64 chips)
        {
            this.Name = playerName;
            this.SeatNumber = seatNumber;
            this.Chips = chips;
            this.Stack = chips;
        }

        public Seat(int seatNumber, string playerName, UInt64 chips, IPlayer brain)
        {
            this.Name = playerName;
            this.SeatNumber = seatNumber;
            this.Chips = chips;
            this.Stack = chips;
            this.Brain = brain;
        }
    }
}
