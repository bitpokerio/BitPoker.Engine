using System;
using System.Collections.Generic;
using System.Text;

namespace BitPoker.Engine
{
    /// <summary>
    /// A class to represent a player's action, like a player raising or folding.
    ///
    /// Author: Wesley Tansey
    /// </summary>
    public class Action : IEquatable<Action>
    {
        #region ActionTypes Enumeration
        public enum ActionTypes
        {
            None,
            PostAnte,
            PostSmallBlind,
            PostBigBlind,
            Fold,
            Check,
            Call,
            Bet,
            Raise
        }
        #endregion

        #region Properties

        /// <summary>
        /// The name of the player performing this action
        /// </summary>
        public String PublicKey { get; set; }
        
        /// <summary>
        /// The type of action that the player is performing
        /// </summary>
        public ActionTypes ActionType { get; set; }
        
        /// <summary>
        /// The amount of money that was committed in this action, or 0 if the
        /// player performed an action that did not require money.
        /// </summary>
        public UInt64 Amount { get; set; }

        /// <summary>
        /// True if this action has put the player all-in.
        /// </summary>
        public bool AllIn { get; set; }
        
        public String Signature { get; private set; }
        
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor, sets name to "", type to Predeal, and amount to 0.
        /// </summary>
        public Action()
        {
            PublicKey = "";
            this.ActionType = ActionTypes.None;
            this.Amount = 0;
        }

        /// <summary>
        /// Constructor for actions which take no money to perform.  Amount is set to 0.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="type"></param>
        public Action(String publicKey, ActionTypes type)
        {
            this.PublicKey = publicKey;
            this.ActionType = type;
            this.Amount = 0;
        }

        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        public Action(string name, ActionTypes type, UInt64 amount)
        {
            this.PublicKey = name;
            this.ActionType = type;
            this.Amount = amount;
        }

        public Action(string name, ActionTypes type, UInt64 amount, bool allIn)
        {
            this.PublicKey = name;
            this.ActionType = type;
            this.Amount = amount;
            this.AllIn = allIn;
        }
        #endregion
        
        
        public void Sign(String privateKey)
        {
            this.Signature = "";
        }

        #region Operator Overloads
        public bool Equals(Action a)
        {
            return ActionType == a.ActionType && PublicKey.CompareTo(a.PublicKey) == 0;
        }
        #endregion

        #region String Conversion Methods
        
        public override string ToString()
        {
            return string.Format("[Action: PublicKey={0}, ActionType={1}, Amount={2}, AllIn={3}]", PublicKey, ActionType, Amount, AllIn);
        }
        
        /// <summary>
        /// Converts the action object to its string representation.  The strings conform
        /// to PokerStars hand _history conventions as of August 02, 2008.
        /// </summary>
        /// <returns></returns>
        public string ToPokerStarsString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("{0}: ", PublicKey);
            
            switch( this.ActionType )
	        {
                case ActionTypes.PostAnte: result.Append("posts the ante $" + this.Amount);
                    break;
                case ActionTypes.PostSmallBlind: result.Append("posts small blind [$" + Amount + "]");
    		        break;
                case ActionTypes.PostBigBlind: result.Append("posts big blind [$" + Amount + "]");
    		        break;
    	        case ActionTypes.Fold: result.Append("folds");
    		        break;
    	        case ActionTypes.Check: result.Append("checks");
    		        break;
                case ActionTypes.Call: result.Append("calls [$" + Amount + "]");
    		        break;
                case ActionTypes.Bet: result.Append("bets [$" + Amount + "]");
    		        break;
                case ActionTypes.Raise: result.Append("raises [$" + Amount + "]");
    		        break;
	        }

            if(this.AllIn)
                result.Append(" and is all-In.");

            return result.ToString();
        }

        #endregion
    }
}
