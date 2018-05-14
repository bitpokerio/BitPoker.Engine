using System;
namespace holdem_engine
{
    public class TableManger
    {
        public Seat[] Seats { get; private set; }

        private readonly HandEngine engine;
    
        public TableManger(Int16 maxPlayers)
        {
            this.Seats = new Seat[maxPlayers];

            //
            this.engine = new HandEngine();
        }
        
        /// <summary>
        /// Seat new player
        /// </summary>
        public void Seat()
        {
            
        }
    }
}
