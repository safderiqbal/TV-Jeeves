using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TVJeeves.Core.BusinessLogic;
using TVJeeves.Core.Entities;

namespace TVJeeves.Dialog
{
    [Serializable]
    public class MainDialog
    {
        public static string optionsString="\n\nI can show you what's on your _favourite_ channels, *suggest* something based on what you are watching or give you a *surprise*.\n\nWhat would you like to do?";
        public static string welcomeGreeting = "Greetings, I'm *Jeeves*. I'll help you decide what to watch on TV." + optionsString;
        public static string baseGreeting = "I'm not sure what you mean by that." + optionsString + "\n\nSay favourite, suggest or surprise.";

        public static readonly IDialog<string> dialog = Chain.PostToChain()
           .Select(msg => msg.Text)
           .Switch(

               new RegexCase<IDialog<string>>(new Regex("^favourite", RegexOptions.IgnoreCase), (context, txt) =>
               {
                   return Chain.ContinueWith(new QuickCategoryDialog(),
                          async (ctx, res) =>
                          {
                              await res;
                              return Chain.Return(baseGreeting);
                           });
               }),
               new RegexCase<IDialog<string>>(new Regex("^suggest", RegexOptions.IgnoreCase), (context, txt) =>
               {
                   return Chain.ContinueWith(new SuggestionDialog(),
                          async (ctx, res) =>
                          {
                              await res;
                              return Chain.Return(baseGreeting);
                          });
               }),
               new RegexCase<IDialog<string>>(new Regex("^hi|hello|greetings", RegexOptions.IgnoreCase), (c, txt) =>
               {
                   return
                   Chain.From(() => new PromptDialog.PromptString("Hi. What channel are you watching?",
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
                       Channel output;

                       if (!channel.Item2.Any())
                           return Chain.Return($"Cannot find channel {channel.Item1}");

                       return Chain.Return($"you are watching {channel.Item1}");
                   });
               }),
               new RegexCase<IDialog<string>>(new Regex("^surprise", RegexOptions.IgnoreCase), (context, txt) =>
               {
                   return Chain.ContinueWith(new SurpriseDialog(),
                          async (ctx, res) =>
                          {
                              await res;
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