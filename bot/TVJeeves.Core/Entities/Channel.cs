using System;
using System.Collections.Generic;

namespace TVJeeves.Core.Entities
{
    [Serializable]
    public class Channel
    {
        public int channelno { get; set; }
        //public int epggenre { get; set; }
        public string title { get; set; }
        //public int channeltype { get; set; }
        public int channelid { get; set; }
        public List<genre> genre { get; set; }
    }
}
