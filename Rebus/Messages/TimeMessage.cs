using System;

namespace Messages
{
    public class TimeMessage
    {
        public TimeMessage(DateTimeOffset time)
        {
            Time = time;
        }

        public DateTimeOffset Time { get; }
    }
}
