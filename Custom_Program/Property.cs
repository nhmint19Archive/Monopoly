using System;
using System.Collections.Generic;
using System.IO;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Property is a land with houses and rents
    /// </summary>
    public class Property : BuyableCell
    {
        private const int MaxHouse = 4;
        private int _type; // group that the property belongs to
        private int _rentHotel, _housePrice, _hotelPrice;
        private int _houses; // the number of houses built on the land, hotel is considered the last house
        // a list that holds the number of properties in the board categorized by type
        private static Dictionary<int, int> _typeRecords = new Dictionary<int, int>();
        private void UpdateTypeRecord()
        {
            if (_typeRecords.ContainsKey(_type))
                _typeRecords[_type]++; // increases the number of properties with same type by 1
            else
                _typeRecords.Add(_type, 1); // add new type 
        }
        // a list that holds the images of Properties based on their properties' types
        private static Dictionary<int, string> _propertiesImgRegistry = new Dictionary<int, string>();
        // register which bitmap is for which property type
        public static void RegisterImg(int type, string filename)
        {
            _propertiesImgRegistry[type] = filename;
        }
        public Property(float x, float y, string name, Bitmap img, Board board) : base(x, y, name, img, board) {
            _rentPrices = new int[MaxHouse];
            _houses = 0;
        }
        public int RentHotel
        {
            get { return _rentHotel; }
        }
        public int HousePrice
        {
            get { return _housePrice; }
        }
        public int HotelPrice
        {
            get { return _hotelPrice; }
        }
        public int Houses
        {
            get { return _houses; }
            set { _houses = value; }
        }
        public int Type
        {
            get { return _type; }
        }
        public override int Value
        {
            get { return _price + _housePrice * _houses; }
        }
        public override int CurrentRent
        {
            get 
            {
                int rent = 0;
                if (OwnedBy != null)
                {
                    if (_houses == MaxHouse) 
                        rent = _rentHotel; // the hotel rent price
                    else 
                        rent = _rentPrices[_houses]; // the house rent price
                    // if all properties with same type belong to one owner, the rent is double
                    if (OwnedBy.GetLands<Property>().FindAll(p => p.Type == _type).Count == _typeRecords[_type])
                        rent *= 2;
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
                    res += "Rent With " + i + " Houses: " + _rentPrices[i] + "\n";
                }
                res += "Property Price: " + _price + "\n"
                    + "House Price: " + _housePrice + "\n"
                    + "Hotel Price: " + _hotelPrice;
                return res;
            }
        }
        public override string OnPlayerEnter(Player player) {
            string res = base.OnPlayerEnter(player);
            if (OwnedBy == player)
            {
                if (_houses == MaxHouse - 1)
                {
                    if (player.Money >= _hotelPrice)
                    {
                        // player can buy a hotel 
                        player.BuildStrategy(this, _hotelPrice);
                        return "Do you want to build a hotel?\nPrice: " + _hotelPrice + "$";
                    }
                    // player does not have enough money to buy a hotel
                    player.EndTurn();
                    return player.Name + " does not have money to buy a hotel";
                }
                else if (_houses < MaxHouse - 1)
                {
                    if (player.Money >= _housePrice)
                    {
                        // player can buy a house
                        player.BuildStrategy(this, _housePrice);
                        return "Do you want to build a house?\nPrice: " + _housePrice + "$";
                    }
                    // player does not have enough money to buy a house
                    player.EndTurn();
                    return player.Name + " does not have money to buy a house";
                }
                // player does not have anything to build
                player.EndTurn();
                return "Cannot build more!";
            }
            return res;
        }
        public override void Reset()
        {
            base.Reset();
            _houses = 0;
        }
        // load the properties information from the database
        public override void Load(QueryResult qr)
        {
            _type = qr.QueryColumnForInt(2);
            UpdateTypeRecord();
            _img = SplashKit.LoadBitmap(Name, _propertiesImgRegistry[_type]);
            _price = qr.QueryColumnForInt(3);
            _rentPrices[0] = qr.QueryColumnForInt(4);
            _rentPrices[1] = qr.QueryColumnForInt(5);
            _rentPrices[2] = qr.QueryColumnForInt(6);
            _rentPrices[3] = qr.QueryColumnForInt(7);
            _rentHotel = qr.QueryColumnForInt(8);
            _housePrice = qr.QueryColumnForInt(9);
            _hotelPrice = qr.QueryColumnForInt(10);
        }
        public override void Draw()
        {
            base.Draw();
            DrawHouse();
        }
        // Draw houses on the property
        private void DrawHouse()
        {
            if (OwnedBy != null)
            {
                // the position of the center of house drawing
                float hX = X + 10;
                float hY = Y + Img.Height - 10;
                // the center of the cell
                float cX = X + Img.Width / 2;
                float cY = Y + Img.Height / 2;
                if (_houses == 0) // indicates the owner
                {
                    Point2D pt1 = GameUtilities.FindRotatePoint(cX, cY, hX, hY - 6, Angle);
                    Point2D pt2 = GameUtilities.FindRotatePoint(cX, cY, hX - 6, hY + 3, Angle);
                    Point2D pt3 = GameUtilities.FindRotatePoint(cX, cY, hX + 6, hY + 3, Angle);
                    SplashKit.FillTriangle(OwnedBy.Color, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y);
                }
                else if (_houses < MaxHouse) // indicates the houses
                {
                    for (int i = 0; i < _houses; i++)
                    {
                        Point2D center = GameUtilities.FindRotatePoint(cX, cY, hX + 12 * i, hY, Angle);
                        SplashKit.FillCircle(OwnedBy.Color, center.X, center.Y, 5);
                    }
                }
                else if (_houses == MaxHouse) // indicates the hotel
                { 
                    Point2D pt1 = GameUtilities.FindRotatePoint(cX, cY, hX - 5, hY - 5, Angle);
                    Point2D pt2 = GameUtilities.FindRotatePoint(cX, cY, hX + 5, hY - 5, Angle);
                    Point2D pt3 = GameUtilities.FindRotatePoint(cX, cY, hX - 5, hY + 5, Angle);
                    Point2D pt4 = GameUtilities.FindRotatePoint(cX, cY, hX + 5, hY + 5, Angle);
                    SplashKit.FillQuad(OwnedBy.Color, new Quad() { Points = new Point2D[] { pt1, pt2, pt3, pt4 } });
                }
            }
        }
    }
}
