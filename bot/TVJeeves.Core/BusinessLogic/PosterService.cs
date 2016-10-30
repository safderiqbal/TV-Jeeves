using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using TVJeeves.Core.Entities;

namespace TVJeeves.Core.BusinessLogic
{
    public class PosterService
    {
        protected string ApiEndpoint { get; set; }

        public PosterService()
        {
            ApiEndpoint = "http://hack2016.trivv.io//omdb/poster/";
        }

        public Poster Get(string title)
        {
            var poster = new Poster();

            try
            {
                var client = new WebClient();
                var content = client.DownloadString(ApiEndpoint + title);
                poster = JsonConvert.DeserializeObject<Poster>(content);
            } catch (Exception e) { }

            return poster;
        }
    }
}
