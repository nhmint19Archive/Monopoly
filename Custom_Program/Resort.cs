using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Resort is a land without houses and has special rules
    /// </summary>
    public class Resort : BuyableCell
    {
        private const int MaxResort = 4;
        public Resort(float x, float y, string name, Bitmap img, Board board) : base(x, y, name, img, board) 
        {
            _rentPrices = new int[MaxResort];
        }
        public override int CurrentRent 
        { 
            get
            {
                int rent = 0;
                if (OwnedBy != null)
                {
                    // the number of resorts with same onwer
                    int numResorts = OwnedBy.GetLands<Resort>().Count;
                    if (numResorts <= MaxResort)
                        rent = _rentPrices[numResorts - 1];
                    else
                        rent =  _rentPrices[MaxResort - 1];
                    if (_board.FesCell == this)
                        rent *= 2;
                }
                return rent; 
            }
        }
        public override string Description
        {
            get
            {
                string res = "Current Rent: " + CurrentRent + "\n";
                for (int i = 0; i < _rentPrices.Length; i++)
                {
                    res += "Rent With " + i + " Resort: " + _rentPrices[i] + "\n";
                }
                res += "Resort Price: " + _price;
                return res;
            }
        }
        public override string OnPlayerEnter(Player player)
        {
            string res = base.OnPlayerEnter(player);
            if (OwnedBy == player)
            {
                player.EndTurn();
                return "Cannot build more!";
            }
            return res;
        }
        public override void Load(QueryResult qr)
        {
            _price = qr.QueryColumnForInt(3);
            _rentPrices[0] = qr.QueryColumnForInt(4);
            _rentPrices[1] = qr.QueryColumnForInt(5);
            _rentPrices[2] = qr.QueryColumnForInt(6);
            _rentPrices[3] = qr.QueryColumnForInt(7);
        }
        public override void Draw()
        {
            base.Draw();
            DrawOwner();
        }
        // draw the owner symbol of the resort
        private void DrawOwner()
        {
            if (OwnedBy != null)
            {
                // the position of the center of house drawing
                float hX = X + 10;
                float hY = Y + Img.Height - 10;
                // the center of the cell
                float cX = X + Img.Width / 2;
                float cY = Y + Img.Height / 2;
                Point2D pt1 = GameUtilities.FindRotatePoint(cX, cY, hX, hY - 6, Angle);
                Point2D pt2 = GameUtilities.FindRotatePoint(cX, cY, hX - 6, hY + 3, Angle);
                Point2D pt3 = GameUtilities.FindRotatePoint(cX, cY, hX + 6, hY + 3, Angle);
                SplashKit.FillTriangle(OwnedBy.Color, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y);
            }
        }
    } 
}
