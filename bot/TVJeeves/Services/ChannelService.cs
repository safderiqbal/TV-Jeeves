﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TVJeeves.Core.Entities;

namespace TVJeeves.Core.BusinessLogic
{
    public class ChannelService
    {
        protected string ApiEndPoint { get; set; }

        public ChannelService()
        {
            ApiEndPoint = "http://hack2016.trivv.io/sky/";
        }

        public List<Channel> Get(string channel)
        {
            var client = new WebClient();
            var content = client.DownloadString(ApiEndPoint + channel);
            var channels = JsonConvert.DeserializeObject<List<Channel>>(content);
            return channels;
        }
    }
}
