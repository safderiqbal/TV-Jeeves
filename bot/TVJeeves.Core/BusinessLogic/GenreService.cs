using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using TVJeeves.Core.Entities;

namespace TVJeeves.Core.BusinessLogic
{
    public class GenreService
    {
        protected string ApiEndpoint { get; set; }

        public GenreService()
        {
            ApiEndpoint = "http://hack2016.trivv.io/sky/genre/";
        }

        public List<TVShow> Get(string genreId)
        {
            var shows = new List<TVShow>();

            try
            {
                var client = new WebClient();
                var content = client.DownloadString(ApiEndpoint + genreId);
                shows = JsonConvert.DeserializeObject<List<TVShow>>(content);
            } catch (Exception e) { }

            return shows;
        }
    }
}
