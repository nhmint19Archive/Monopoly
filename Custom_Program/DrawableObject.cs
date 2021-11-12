using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Every objects with drawing ability inherit this
    /// </summary>
    public abstract class DrawableObject
    {
        private float _x, _y; // the position on screen
        public DrawableObject(float x, float y)
        {
            _x = x;
            _y = y;
        }
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }
        public abstract void Draw();
    }
}
