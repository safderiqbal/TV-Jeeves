﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TVJeeves.Base.Entities
{
    public class Suggestion
    {
        public string EventId { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Channel { get; set; }

        public string GenreId { get; set; }
        public string SubGenreId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Description { get; set; }
    }
}
