using System;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// Useful methods for creating games
    /// </summary>
    public static class GameUtilities
    {
        // Delay the screen for some action
        public static void ScreenDelay(uint time, Action action)
        {
            action();
            ScreenDelay(time);
        }
        // Delay the screen for a specified time
        public static void ScreenDelay(uint time)
        {
            SplashKit.RefreshScreen();
            SplashKit.Delay(time);
        }
        // Find a point when rotate a point around center point
        public static Point2D FindRotatePoint(float centerX, float centerY, float x, float y, float angle)
        {
            /* The formula for this is 
             * newX = (x - centerX)cosa - (y - centerY)sina
             * newY = (x - centerX)sina + (y - centerY)cosa
             */
            float newX = (x - centerX) * SplashKit.Cosine(angle) - (y - centerY) * SplashKit.Sine(angle) + centerX;
            float newY = (x - centerX) * SplashKit.Sine(angle) + (y - centerY) * SplashKit.Cosine(angle) + centerY;
            return SplashKit.PointAt(newX, newY);
        }
    }
}
