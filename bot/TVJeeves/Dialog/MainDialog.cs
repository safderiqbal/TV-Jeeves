using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

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
                              return Chain.ContinueWith(new SuggestionDialog(),
                                  async (ctx1, res1) =>
                                  {
                                      await res1;
                                      return Chain.Return(baseGreeting);
                                  });
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