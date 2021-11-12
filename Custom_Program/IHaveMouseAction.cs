using SplashKitSDK;
using System;

namespace Custom_Program
{
    /// <summary>
    /// The interface that defines objects with mouse click event
    /// </summary>
    public interface IHaveMouseAction
    {
        bool IsAt(Point2D point);
        event EventHandler Clicked;
        void OnClick(EventArgs e);
    }
}
