using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Temporary button is a button that can be disabled and enabled
    /// </summary>
    public class TemporaryButton : Button
    {
        private bool _disabled;
        public TemporaryButton(float x, float y, int width, int height, string name, int textSize, Color color, Color colorOnHover, Color textColor)
             : base(x, y, width, height, name, textSize, color, colorOnHover, textColor) 
        {
            _disabled = true;
        }
        public override void Draw()
        {
            if (!_disabled)
                base.Draw();
        }
        public override bool IsAt(Point2D point)
        {
            return !_disabled && base.IsAt(point);
        }
        public bool Disabled
        {
            get { return _disabled; }
            set { _disabled = value; }
        }
    }
}
