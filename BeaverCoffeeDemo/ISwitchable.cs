using System;


namespace BeaverCoffeeDemo
{
    // Navigation with switchable user controls within one main window is based on:
    // https://azerdark.wordpress.com/2010/04/23/multi-page-application-in-wpf/
    public interface ISwitchable
    {
        void UtilizeState(Object state);
    }
}
