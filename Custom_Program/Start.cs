using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// The starting line of the game
    /// </summary>
    public class Start : Cell
    {
        public Start(float x, float y, string name, Bitmap img, Board board) : base(x, y, name, img, board) { }
        public override string Description
        {
            get { return "Starting line\nWalk past here to earn 300$"; }
        }
        public override string OnPlayerEnter(Player player)
        {
            return base.OnPlayerEnter(player) + player.Name + " has arrived at the Start Line";
        }
    } 
}
