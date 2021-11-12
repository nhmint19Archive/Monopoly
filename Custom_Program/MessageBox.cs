using System;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// MessageBox is a special box used for expressing messages in game.
    /// </summary>
    public class MessageBox : DrawableObject
    {
        private string _msg;
        private int _textSize;
        protected int _width;
        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }
        public MessageBox(float x, float y, int width, int textSize) : base(x, y) {
            _textSize = textSize;
            _msg = "";
            _width = width;
        }
        public override void Draw()
        {
            int msgWidth, msgHeight;
            string[] msgLines = _msg.Split('\n'); // Draw text by splitting into lines
            for (int i = 0; i < msgLines.Length; i++)
            {
                msgWidth = SplashKit.TextWidth(msgLines[i], "GameFont", _textSize);
                msgHeight = SplashKit.TextHeight(msgLines[i], "GameFont", _textSize);
                SplashKit.DrawText(msgLines[i], Color.Black, "GameFont", _textSize, X + (_width - msgWidth) / 2, Y + msgHeight * i);
            }
        }
    }
}
