using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVJeeves.Core.Entities
{
    public class TVShow
    {
        public int eventid { get; set; }

        public int channelid { get; set; }

        public string date { get; set; }

        public DateTime startAsDateTime
        {
            get
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddMilliseconds(Convert.ToDouble(start));
            }
        }

        public string start { get; set; }

        public int dur { get; set; }

        public string title { get; set; }

        public string shortDesc { get; set; }

        public int genre { get; set; }

        public int subgenre { get; set; }

        public string scheduleStatus { get; set; }

        public Channel channel {get;set;}

        public string imageurl { get; set; }
    }
}
