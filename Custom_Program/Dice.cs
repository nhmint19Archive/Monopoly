using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Dice's main job is to give the value of it
    /// </summary>
    public class Dice : DrawableObject
    {
        private Animation _anim;
        private Bitmap _img;
        private int _value;
        private bool _disabled; // dice can be disabled
        private bool _isRolling; // check if dice is rolling or not
        public Dice(float x, float y, Bitmap img) : base(x, y)
        {
            _value = 0;
            _img = img;
            _disabled = false;
            _isRolling = false;
        }
        public Bitmap Img
        {
            get { return _img; }
        }
        public Animation Anim
        {
            get { return _anim; }
            set { _anim = value; }
        }
        public int Value
        {
            get { return _value; }
        }
        public bool Disabled
        {
            get { return _disabled; }
            set { _disabled = value; }
        }
        public bool EndRolling()
        {
            if (_isRolling && Anim.Ended)
            {
                _isRolling = false;
                return true;
            }
            return false;
        }
        // dice rolls 
        public void Roll() {
            _isRolling = true;
            _value = new Random().Next(1, 7);
            _anim.Assign(_value);
        }
        // reset the dice
        public void Reset()
        {
            _value = 0;
            _disabled = true;
        }
        public override void Draw()
        {
            if (!_disabled)
            {
                // create Drawing Options with Animation
                DrawingOptions diceOpt = SplashKit.OptionWithAnimation(_anim);
                SplashKit.DrawBitmapOnWindow(SplashKit.CurrentWindow(), _img, X, Y, diceOpt);
                _anim.Update();
            }
        }
    }
}
