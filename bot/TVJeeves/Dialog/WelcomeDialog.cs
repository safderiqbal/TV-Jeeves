using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;

namespace TVJeeves.Dialog
{
    public class WelcomeDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //do something with context
        }

        public static readonly IDialog<string> dialog = Chain
            .PostToChain()
            .Switch(
                new Case<IMessageActivity, IDialog<string>>((msg) =>
                {
                    var regex = new Regex("^hi|hello|greetings", RegexOptions.IgnoreCase);
                    return regex.IsMatch(msg.Text);
                }, (ctx, msg) =>
                {
                    return 
                    Chain.From(() => new PromptDialog.PromptString("Welcome! My name is Jeeves and I will guide you today. What channel are you watching?",
"Didn't get that!", 3))
                    .ContinueWith(async (context, response) =>
                    {
                        if(!string.IsNullOrEmpty(await response))
                        {
                            return Chain.Return("Are you watching " + await response);
                        }
                        return Chain.Return("could not get your response");
                    });
                }
                ),
                new DefaultCase<IMessageActivity, IDialog<string>>((ctx, msg) =>
                {
                    return Chain.Return("Sorry, I didn't understand that.");
                })).Unwrap().PostToUser();
    }
}