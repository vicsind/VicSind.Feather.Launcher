using System.Collections.Generic;

namespace Updater.Content
{
    public class Rating
    {
        public string Name { get; set; }
        public IEnumerable<RatingItem> Players { get; set; }
    }
}
