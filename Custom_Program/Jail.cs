using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Jail is a cell that keeps player from moving for 3 turns
    /// </summary>
    public class Jail : Cell
    {
        public Jail(float x, float y, string name, Bitmap img, Board board) : base(x, y, name, img, board) { }
        public override string OnPlayerEnter(Player player)
        {
            base.OnPlayerEnter(player);
            player.TurnsInJail = 3;
            player.SameDice = 0; // the same dice number does not matter when player is in jail
            return player.Name + " has been sentenced in Jail.";
        }
        public override string Description
        {
            get 
            {
                return  "Player is kept in jail for 3 turns if cannot" +
                        "\nroll the same number or pay 100$ to get out" +
                        "\nPlayer can also be in jail if rolling \nthe same dice number for 3 times";
            }
        }
    } 
}
