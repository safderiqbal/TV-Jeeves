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

            cc.Type = "message";
            cc.Attachments = new List<Attachment>();

            var poster = new PosterService().Get(tvShow.title);
            var imgUrl = poster != null && poster.poster != null ? poster.poster : "https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcRj0YzDMrnC8kqGjvTH3tQ_VpVY4HbtcpGCNcJ_tR4WdiMKvjYc";

            var plCard = new ThumbnailCard()
            {
                Title = tvShow.title,
                Subtitle = tvShow.channel.title + " (" + tvShow.channel.channelid  + ") - " + tvShow.startAsDateTime.ToString(),
                Text = $"{tvShow.shortDesc} - {tvShow.startAsDateTime.ToString()}",
                Images = new List<CardImage> { new CardImage(url: imgUrl) }
            };
            Attachment plAttachment = plCard.ToAttachment();
            cc.Attachments.Add(plAttachment);

            //cc.Text += $"{tvShow.channel.channelno}. {tvShow.channel.title} **{tvShow.title}** {tvShow.shortDesc} on at *{tvShow.startAsDateTime.ToString()}*";
            await context.PostAsync(cc);
            //context.Reset();
            context.Wait(HandleSuggestionResponse);
        }

        public async Task HandleSuggestionResponse(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            //await argument;
            context.Done(this);
        }
    }
}