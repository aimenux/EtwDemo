using System;
using System.Diagnostics.Tracing;

namespace EtwNetFull.Providers
{
    [EventSource(Name="ETW-Demo-Custom")]
    public sealed class CustomEventSource : EventSource
    {
        public static CustomEventSource Log = new CustomEventSource();

        [Event(1, Message="Message submitted: {0} at {1}", Channel = EventChannel.Debug)]
        public void MessageSubmitted(string message, DateTime date)
        {
            WriteEvent(1, message, date);
        }
    }
}
