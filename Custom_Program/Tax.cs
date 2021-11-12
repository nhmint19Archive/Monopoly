using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Tax is a cell which forces player to pay rents
    /// </summary>
    public class Tax : Cell
    {
        public Tax(float x, float y, string name, Bitmap img, Board board) : base(x, y, name, img, board) { }
        public override string OnPlayerEnter(Player player)
        {
            int tax = player.TotalMoney * 10 / 100;
            if (player.Money < tax)
            {
                // player's money is less than tax
                player.SellStrategy(null, tax);
                return "You have to sell to pay the debt\nPay Total: " + tax + "$\nSell Total: " + player.SellTotal + "$";
            }
            // player pays the tax
            player.Pay(null, tax);
            player.EndTurn();
            return player.Name + " has paid " + tax+ "$";
        }
        public override string Description
        {
            get
            {
                return "Player has to pay 10% of total money";
            }
        }
    } 
}
