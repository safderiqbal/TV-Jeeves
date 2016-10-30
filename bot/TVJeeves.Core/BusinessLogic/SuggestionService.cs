using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TVJeeves.Base.Entities;

namespace TVJeeves.Base.BusinessLogic
{
    public class SuggestionService
    {
        public List<Suggestion> GetUserSuggestions()
        {
            return new List<Suggestion>()
            {
                new Suggestion() {Name="Breaking Bad", ImageUrl="http://images.zap2it.com/assets/p185846_b_h3_ad/breaking-bad.jpg", Channel="Sky One", StartTime=DateTime.Now },
                new Suggestion() {Name="Game of Thrones", ImageUrl="http://media.moddb.com/images/members/1/123/122021/profile/c9lzmv4d3mgzpnyntz7s.jpg", Channel="Sky Atlantic", StartTime=DateTime.Now }
            };
        }
        public List<Suggestion> GetProgrammesOnChannels(List<int> Channels)
        {
            return new List<Suggestion>()
            {
                new Suggestion() {Name="Premier League Football", ImageUrl="", Channel="Sky Sports 1", StartTime=DateTime.Now, EndTime=DateTime.Now.AddHours(1) },
                new Suggestion() {Name="Premier League Darts", ImageUrl="", Channel="Sky Sports 2", StartTime=DateTime.Now, EndTime=DateTime.Now.AddHours(1).AddMinutes(-15) },
                new Suggestion() {Name="Superleague Rugby", ImageUrl="", Channel="Sky Sports 3", StartTime=DateTime.Now, EndTime=DateTime.Now.AddHours(1).AddMinutes(-15) },
                new Suggestion() {Name="Bobsleighing", ImageUrl="", Channel="Eurosports", StartTime=DateTime.Now, EndTime=DateTime.Now.AddHours(1).AddMinutes(-15) },
                new Suggestion() {Name="NFL Football : New York Jets v San Fransisco 49ers", ImageUrl="", Channel="Eurosports 2", StartTime=DateTime.Now, EndTime=DateTime.Now.AddHours(1).AddMinutes(-15) }
            };
        }
    }
}
