using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Buttons in this program are rectangles with click events
    /// </summary>
    public class Button : DrawableObject, IHaveMouseAction
    {
        private int _width, _height;
        private Color _color, _colorOnHover, _textColor;
        private string _name;
        private int _textSize;
        public Button(float x, float y, int width, int height, string name, int textSize, Color color, Color colorOnHover, Color textColor) : base(x, y)
        {
            _width = width;
            _height = height;
            _color = color;
            _colorOnHover = colorOnHover;
            _textColor = textColor;
            _name = name;
            _textSize = textSize;
        }
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public string Name
        {
            get { return _name; }
        }
        public override void Draw()
        {
            if (IsAt(SplashKit.MousePosition()))
                SplashKit.FillRectangle(_colorOnHover, X, Y, _width, _height);
            else
                SplashKit.FillRectangle(_color, X, Y, _width, _height);
            DrawText();
        }
        // Draw the text on the button
        private void DrawText()
        {
            float textX = X + (_width - SplashKit.TextWidth(Name, "GameFont", _textSize)) / 2;
            float textY = Y + (_height - SplashKit.TextHeight(Name, "GameFont", _textSize)) / 2;
            SplashKit.DrawText(Name, _textColor, "GameFont", _textSize, textX, textY);
        }
        public virtual bool IsAt(Point2D point)
        {
            return SplashKit.PointInRectangle(
                point, new Rectangle() { X = X, Y = Y, Width = _width, Height = _height }
            );
        }
        public event EventHandler Clicked;
        public void OnClick (EventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
    }
}
