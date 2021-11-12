using System;
using System.Collections.Generic;
using System.IO;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Represents the cell on the board
    /// </summary>
    public abstract class Cell : DrawableObject
    {
        private string _name; // the name of the cell
        protected Bitmap _img; // the image of the cell
        private float _angle; // the angle that the image will rotate on the board
        private bool _isSelected; // check if the cell is clicked or not
        private int _position; // the position of the sell on the board
        protected Board _board;
        public Cell(float x, float y, string name, Bitmap img, Board board) : base(x, y) {
            _name = name;
            _img = img;
            _angle = 0;
            _isSelected = false;
            _position = 0;
            _board = board;
        }
        public string Name
        {
            get { return _name; }
        }
        public Bitmap Img
        {
            get { return _img; }
        }
        public float Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }
        // the surrounding quadilateral of the cell
        private Quad SurroundingQuad
        {
            get
            {
                float centerX = X + Img.Width / 2;
                float centerY = Y + Img.Height / 2;
                Point2D pt1 = GameUtilities.FindRotatePoint(centerX, centerY, X - 1, Y - 1, _angle);
                Point2D pt2 = GameUtilities.FindRotatePoint(centerX, centerY, X + Img.Width, Y - 1, _angle);
                Point2D pt3 = GameUtilities.FindRotatePoint(centerX, centerY, X - 1, Y + Img.Height, _angle);
                Point2D pt4 = GameUtilities.FindRotatePoint(centerX, centerY, X + Img.Width, Y + Img.Height, _angle);
                return new Quad() { Points = new Point2D[] { pt1, pt2, pt3, pt4 } };
            }
        }
        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }
        // rules when a player enter the cell
        public virtual string OnPlayerEnter(Player player)
        {
            player.IsInTurn = false;
            return "";
        }
        public override void Draw()
        {
            SplashKit.DrawBitmapOnWindow(SplashKit.CurrentWindow(), _img, X, Y, SplashKit.OptionRotateBmp(_angle));
            if (IsSelected || IsAt(SplashKit.MousePosition()))
                DrawOutline(Color.Black);
        }
        // draw the outline of the cell
        public void DrawOutline(Color color) => SplashKit.DrawQuad(color, SurroundingQuad);
        // check if the cell is in a specific location on the window
        public bool IsAt(Point2D point)
        {
            return SplashKit.PointInQuad( point, SurroundingQuad );
        }
        // load cell information from the database
        public virtual void Load(QueryResult qr) { }
        // the short description of the cell
        public virtual string Description
        {
            get { return ""; }
        }

    }
}
