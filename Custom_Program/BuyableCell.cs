using System;
using System.Collections.Generic;
using System.IO;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// The cell that can be bought by players e.g properties and resorts
    /// </summary>
    public abstract class BuyableCell : Cell
    {
        private Player _ownedBy; // the owner of the cell
        protected int[] _rentPrices; // the rent values
        protected int _price; // the original price of the cell
        public BuyableCell(float x, float y, string name, Bitmap img, Board board) : base(x, y, name, img, board) {
            _ownedBy = null;
        }
        public int Price
        {
            get { return _price; }
        }
        public Player OwnedBy
        {
            get { return _ownedBy; }
            set { _ownedBy = value; }
        }
        // get the true value of the land
        public virtual int Value
        {
            get { return _price; }
        }
        // get the current rent of the land
        public virtual int CurrentRent
        {
            get { return 0; }
        }
        // reset the land to unowned land
        public virtual void Reset()
        {
            _ownedBy = null;
        }
        public override string OnPlayerEnter(Player player)
        {
            if (OwnedBy == null)
            {
                if (player.Money >= _price)
                {
                    // player can buy this resort
                    player.BuyStrategy(this);
                    return "Do you want to buy this land?\nPrice: " + _price + "$";
                }
                // player does not have enough money to buy this resort
                player.EndTurn();
                return player.Name + " cannot afford this land";
            }
            else if (OwnedBy != player)
            {
                // player has to pay money to the owner
                if (player.Money < CurrentRent)
                {
                    player.IsSelling = true;
                    // player has to sell properties to pay the rent
                    if (player.TotalMoney >= CurrentRent)
                    {
                        player.SellStrategy(OwnedBy, CurrentRent);
                        return "You have to sell to pay the rent\nPay Total: " + CurrentRent + "$\nSell Total: " + player.SellTotal + "$";
                    }
                    // player has to sell all and go bankrupt
                    player.SellAll();
                    int payment = player.Money;
                    player.Pay(OwnedBy, payment);
                    player.EndTurn();
                    player.IsBankrupt = true;
                    return player.Name + " has paid " + payment + "$\n" + player.Name + " is bankrupt!";
                }
                // player pays the rent
                player.Pay(OwnedBy, CurrentRent);
                player.EndTurn();
                return player.Name + " has paid " + CurrentRent + "$";
            }
            return "";
        }
        public override void Draw()
        {
            base.Draw();
            DrawName();
        }
        // draw the land's name to the cell
        private void DrawName()
        {
            int nameWidth = SplashKit.TextWidth(Name, "CellFont", 13);
            SplashKit.DrawTextOnBitmap(_img, Name, Color.Black, "CellFont", 13, (_img.Width - nameWidth) / 2, 10);
        }
    }
}
