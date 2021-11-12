using System;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Festival is a cell that allows player to host a festival on the chosen land
    /// </summary>
    public class Festival : Cell
    {
        public Festival(float x, float y, string name, Bitmap img, Board board) : base(x, y, name, img, board) { }
        public override string OnPlayerEnter(Player player)
        {
            if (player.LandCount == 0)
            {
                player.IsInTurn = false;
                return player.Name + " does not have any lands to host a festival";
            }
            player.IsChoosingFes = true;
            return "Choose a land of yours to host the festival";  
        }
        public override string Description
        {
            get { return "Player can choose a land to host a festival\nThe land's rent will raise double\nOnly one land can host a festival"; }
        }
    } 
}
