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
                new Suggestion() {Name="Breaking Bad", ImageUrl="http://images.zap2it.com/assets/p185846_b_h3_ad/breaking-bad.jpg", Channel="Sky One", Time="9PM" },
                new Suggestion() {Name="Game of Thrones", ImageUrl="http://media.moddb.com/images/members/1/123/122021/profile/c9lzmv4d3mgzpnyntz7s.jpg", Channel="Sky Atlantic", Time="9PM" }
            };
        }
    }
}
