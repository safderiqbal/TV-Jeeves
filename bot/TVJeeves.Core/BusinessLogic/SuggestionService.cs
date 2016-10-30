using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TVJeeves.Base.Entities;
using TVJeeves.Core.Entities;

namespace TVJeeves.Base.BusinessLogic
{
    [Serializable]
    public class SuggestionService
    {
        public string ApiEndPoint;

        public static Dictionary<string, TVShow> savedSuggestions = new Dictionary<string, TVShow>();

        public SuggestionService()
        {
            ApiEndPoint = "http://hack2016.trivv.io/sky/channel/current/";
        }

        public List<TVShow> Get(string channel)
        {
            var suggestions = new List<TVShow>();
            try
            {
                var client = new WebClient();
                var content = client.DownloadString(ApiEndPoint + channel);

            suggestions = JsonConvert.DeserializeObject<List<TVShow>>(content);

                foreach (var item in suggestions)
                {
                    savedSuggestions[item.eventid.ToString()] = item;
                }
                /*foreach(var item in jsonResponse)
                {
                    var sg =
                    new Suggestion()
                    {
                        /*EventId = item.eventid,
                        Name = item.title,
                        Channel = item.channelid,
                        GenreId = item.genre,
                        Description = item.shortDesc,
                        SubGenreId = item.subgenre*/
                // };
                //suggestions.Add(sg);
                //savedSuggestions[sg.EventId] = sg;
                // }*/
            }
            catch (Exception e)
            {

            }
            return suggestions;
        }

        public TVShow GetById(string id)
        {
            //ToDo error catching on this
            return savedSuggestions[id];
        }

        public List<TVShow> GetUserSuggestions()
        {
            return new List<TVShow>()
            {
                new TVShow() {
                    title ="Game of Thrones",
                    shortDesc ="First of His Name: Tywin and Cersei plot the Crown's next move, while, in the north, Jon embarks on a new mission. Violence, strong language and adult themes. (S4, ep 5) Also in HD",
                    channel=new Channel {title="Sky Atlantic", channelno=108 },
                    imageurl ="http://media.moddb.com/images/members/1/123/122021/profile/c9lzmv4d3mgzpnyntz7s.jpg",
                    start="1477821000000"
                    },
                new TVShow() { 
                    title ="Breaking Bad",
                    shortDesc ="Walt ties up loose ends, but when confronted with evidence of his success he makes a startling and dangerous decision.",
                    channel=new Channel {title="AMC", channelno=276 },
                    imageurl ="http://images.zap2it.com/assets/p185846_b_h3_ad/breaking-bad.jpg",
                    start="1477821000000"
                }
            };
        }
        public List<TVShow> GetProgrammesOnChannels(string Channels)
        {
            return Get(Channels);// "1301,1302,1333,1322,1303");
            
        }


        //eventid":"283","channelid":"1302","date":"30/10/16","start":"1477792800000","dur":"900","title":"Michael Vaughan: Sporting Triumphs","shortDesc":"A look at the incredible sporting triumph of England captain Michael Vaughan as he led his side to one of the most memorable Ashes series wins of all time. Also in HD","genre":"7","subgenre":"6","edschoice":"false","parentalrating":{"k":"0","v":"--"},"widescreen":"","sound":{"k":"1","v":"Simple stereo"},"remoteRecordable":"false","record":"1","scheduleStatus":"FINISHED","blackout":"false","movielocator":"null"

        /*    [{"channelno":"401","title":"Sky Sports 1","channelid":"1301","epggenre":[{"genreid":"15","name":"Sports"}],"genre":[{"genreid":"7","name":"Sports"}],"channeltype":[{"typeid":"1","name":"SD"}]},
            {"channelno":"402","title":"Sky Sports 2","channelid":"1302","epggenre":[{"genreid":"15","name":"Sports"}],"genre":[{"genreid":"7","name":"Sports"}],"channeltype":[{"typeid":"1","name":"SD"}]},
            {"channelno":"403","title":"Sky Sports 3","channelid":"1333","epggenre":[{"genreid":"15","name":"Sports"}],"genre":[{"genreid":"7","name":"Sports"}],"channeltype":[{"typeid":"1","name":"SD"}]},
            {"channelno":"404","title":"Sky Sports 4","channelid":"1322","epggenre":[{"genreid":"15","name":"Sports"}],"genre":[{"genreid":"7","name":"Sports"}],"channeltype":[{"typeid":"1","name":"SD"}]},
            {"channelno":"405","title":"Sky Sports 5","channelid":"1303","epggenre":[{"genreid":"15","name":"Sports"}],"genre":[{"genreid":"7","name":"Sports"}],"channeltype":[{"typeid":"1","name":"SD"}]}]

            */

            [Serializable]
        public class tBB
        {
            public int eventid { get; set; }
        }

        [Serializable]
        public class tempProgramme
        {
            public int eventid { get; set; }

            public int channelid { get; set; }

            public string date { get; set; }

            public string start { get; set; }

            public string dur { get; set; }

            public string title { get; set; }

            public string shortDesc { get; set; }

            public string genre { get; set; }

            public string subgenre { get; set; }

            public string edschoice { get; set; }

            public tempObject parentalrating { get; set; }//":{"k":"0","v":"--"}

            public string widescreen { get; set; }

            public tempObject sound { get; set; }//":{"k":"1","v":"Simple stereo"}

            public string remoteRecordable { get; set; }

            public string record { get; set; }

            public string scheduleStatus { get; set; }

            public string blackout { get; set; }

            public string movielocator { get; set; }

        }

        public class tempObject
        {
            public string v { get; set; }
            public string k { get; set; }
        }

    }
}