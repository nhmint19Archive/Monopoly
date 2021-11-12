using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Custom_Program
{
    /// <summary>
    /// listen to click event of all IHaveMouseAction objects, based on Observer pattern
    /// </summary>
    public class MouseInputManager
    {
        // a list of observers (objects with mouse action)
        private List<IHaveMouseAction> _observers;
        public MouseInputManager()
        {
            _observers = new List<IHaveMouseAction>();
        }
        // add observer
        public void Add(IHaveMouseAction observer) => _observers.Add(observer);
        // notify all observers to handle all click events at once
        public void NotifyObservers()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                foreach (IHaveMouseAction observer in _observers)
                    if (observer.IsAt(SplashKit.MousePosition()))
                        observer.OnClick(EventArgs.Empty);
            }
        }
        
    }
}
