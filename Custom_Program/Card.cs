using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Cards have special effect on players' gameplay
    /// </summary>
    public abstract class Card
    {
        private string _description;
        public Card()
        {
            _description = "";
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        } 
        // Activate the card effect
        public abstract void Activate(Player player, Board board);
    }
}
