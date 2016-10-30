using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TVJeeves.Core.BusinessLogic;

namespace TVJeeves.Dialog
{
    [Serializable]
    public class SurpriseDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var cc = context.MakeMessage();
            var tvShow = new ShowService().Random();

            cc.Text = "I've selected a surprise show for you...\n";
            cc.Text += $"{tvShow.channel.channelno}. {tvShow.channel.title} **{tvShow.title}** {tvShow.shortDesc} on at *{tvShow.startAsDateTime.ToString()}*";
            await context.PostAsync(cc);
            context.Reset();
        }

        public async Task HandleSuggestionResponse(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            var cc = context.MakeMessage();

            cc.Text = "That works" + message.Text;
            await context.PostAsync(cc);
        }
    }
}