using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TVJeeves.Base.BusinessLogic;
using TVJeeves.Core.BusinessLogic;
using TVJeeves.Core.Entities;

namespace TVJeeves.Dialog
{
    [Serializable]
    public class MainDialog
    {
        public static string optionsString="\n\nI can show you what's on your _favourite_ channels, _suggest_ some popular shows, recommend something based on what you are _watching_ or give you a _surprise_.\n\nWhat would you like to do?";
        public static string welcomeGreeting = "Greetings, I'm *Jeeves*. I'll help you decide what to watch on TV." + optionsString;
        public static string baseGreeting = "I'm not sure what you mean by that." + optionsString + "\n\nSay _favourite_, _suggest_, _watching_ or _surprise_.";

        public static readonly IDialog<string> dialog = Chain.PostToChain()
           .Select(msg => msg.Text)
           .Switch(

               new RegexCase<IDialog<string>>(new Regex("^favourite", RegexOptions.IgnoreCase), (context, txt) =>
               {
                   return Chain.ContinueWith(new QuickCategoryDialog(),
                          async (ctx, res) =>
                          {
                              var message = await res;
                              bool success;

                              if (message.ToLower()=="success")
                              {
                                  return Chain.Return("Enjoy the show! Start chatting again anytime to get another suggestion.");
                              }
                              return Chain.Return(baseGreeting);
                           });
               }),
               new RegexCase<IDialog<string>>(new Regex("^suggest", RegexOptions.IgnoreCase), (context, txt) =>
               {
                   return Chain.ContinueWith(new SuggestionDialog(),
                          async (ctx, res) =>
                          {
                              var message = await res;
                              bool success;

                              if (message.ToLower() == "success")
                              {
                                  return Chain.Return("Enjoy the show! Start chatting again anytime to get another suggestion.");
                              }
                              return Chain.Return(baseGreeting);
                          });
               }),
               new RegexCase<IDialog<string>>(new Regex("(:?.*)?watch(:?.*)?", RegexOptions.IgnoreCase), (c, txt) =>
               {
                   return
                   Chain.From(() => new PromptDialog.PromptString($"Hello. What channel are you watching?",
    "Didn't get that!", 3))
                   .ContinueWith(async (context, response) =>
                   {
                       if (!string.IsNullOrEmpty(await response))
                       {
                           var res = await response;
                           var channels = new ChannelService().Get(res);

                           if (!channels.Any())
                               return Chain.Return(new Tuple<string, List<Channel>>(res, new List<Channel>()));

                           context.UserData.SetValue("channels", channels);
                           context.UserData.SetValue("channelsInd", 0);
                           return Chain.Return(new Tuple<string, List<Channel>>(res, channels));
                       }
                       return Chain.Return(new Tuple<string, List<Channel>>("Error", new List<Channel>()));
                   })
                   .ContinueWith(async (context, response) =>
                   {
                       var channel = (Tuple<string, List<Channel>>)await response;
                       var genre = channel.Item2.First().genre.First();
                       var currentlyOnChannel = new SuggestionService().Get(channel.Item2.First().channelid.ToString()).First();

                       var shows = new GenreService().Get(genre.genreid, currentlyOnChannel.SubGenreId).Where(x => x.scheduleStatus != "PLAYING_NOW").ToList();

                       var output = $"You are currently watching **{currentlyOnChannel.Name}**\n";
                       output += $"Here are some other programs currently showing of the same genre of **{genre.name}** \n";

                       for (int i = 0; i < (shows.Count >= 10 ? 10: shows.Count); i++)
                       {
                           output += $"{shows[i].channel.channelno}. {shows[i].channel.title} {shows[i].title} \n";
                           output += $"**Short Desc** {shows[i].shortDesc} *{shows[i].startAsDateTime}*\n";
                       }

                       return Chain.Return(output);
                   });
               }),
               new RegexCase<IDialog<string>>(new Regex("^surprise", RegexOptions.IgnoreCase), (context, txt) =>
               {
                   return Chain.ContinueWith(new SurpriseDialog(),
                          async (ctx, res) =>
                          {
                              var message = await res;
                              return Chain.Return(baseGreeting);
                          });
               }),
               new DefaultCase<string, IDialog<string>>((context, txt) =>
               {
                   bool welcomeMessageSeen;
                   context.UserData.TryGetValue("welcomeMessageSeen", out welcomeMessageSeen);
                   if (!welcomeMessageSeen)
                   {
                       context.UserData.SetValue("welcomeMessageSeen", true);
                       context.UserData.SetValue("success", false);
                       return Chain.Return(welcomeGreeting);
                   }
                   else
                   {
                       return Chain.Return(baseGreeting);

                   }

               }))
           .Unwrap()
           .PostToUser();   
    }
}