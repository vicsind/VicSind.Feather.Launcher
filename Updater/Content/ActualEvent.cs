using System;

namespace Updater.Content
{
    public class ActualEvent
    {
        public string Name { get; set; }
        public string Map { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
