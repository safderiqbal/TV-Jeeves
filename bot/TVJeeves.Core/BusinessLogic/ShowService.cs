using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using TVJeeves.Core.Entities;

namespace TVJeeves.Core.BusinessLogic
{
    public class ShowService
    {
        protected string ApiEndpoint { get; set; }

        public ShowService()
        {
            ApiEndpoint = "http://hack2016.trivv.io/sky/show/";
        }

        public TVShow Random()
        {
            var show = new TVShow();

            try
            {
                var client = new WebClient();
                var content = client.DownloadString(ApiEndpoint + "random");
                show = JsonConvert.DeserializeObject<TVShow>(content);
            } catch (Exception e) { }

            return show;
        }
    }
}
