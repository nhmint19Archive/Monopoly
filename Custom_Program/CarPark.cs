using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Carpark is a regular cell
    /// </summary>
    public class CarPark : Cell
    {
        public CarPark(float x, float y, string name, Bitmap img, Board board) : base(x, y, name, img, board) { }
        public override string Description
        {
            get { return "Just a normal parking lot"; }
        }
        public override string OnPlayerEnter(Player player)
        {
            return base.OnPlayerEnter(player) + player.Name + " has arrived at the Airport";
        }
    } 
}
