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

        public DateTime date { get; set; }

        public DateTime start { get; set; }

        public int dur { get; set; }

        public string title { get; set; }

        public string shortDesc { get; set; }

        public int genre { get; set; }

        public int subgenre { get; set; }

        public string scheduleStatus { get; set; }
    }
}
