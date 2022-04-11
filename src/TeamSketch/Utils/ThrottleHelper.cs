using System;
using System.Threading.Tasks;
using Avalonia.Input;

namespace TeamSketch.Utils;

public static class ThrottleHelper
{
    public static EventHandler<PointerEventArgs> CreateThrottledEventHandler(
         EventHandler<PointerEventArgs> handler,
         TimeSpan throttle)
    {
        bool throttling = false;
        return (s, e) =>
        {
            if (throttling) return;
            handler(s, e);
            throttling = true;
            Task.Delay(throttle).ContinueWith(_ => throttling = false);
        };
    }
}
