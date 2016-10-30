using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TVJeeves.Base.BusinessLogic;
using TVJeeves.Base.Entities;
using TVJeeves.Core.BusinessLogic;
using TVJeeves.Core.Entities;

namespace TVJeeves.Dialog
{
    [Serializable]
    public class QuickCategoryDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {

            var message = context.MakeMessage();

            message.Type = "message";
            message.Attachments = new List<Attachment>();

            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButton = new CardAction()
            {
                Value = "sports",
                Type = "postBack",
                Title = "Sports"
            };
            cardButtons.Add(plButton);
            plButton = new CardAction()
            {
                Value = "movies",
                Type = "postBack",
                Title = "Movies"
            };
            cardButtons.Add(plButton);


            ThumbnailCard plCard = new ThumbnailCard()
            {
                Title = "",
                Subtitle = "I've selected some of your favourite channels, anything here here interest you?",
                Buttons = cardButtons
            };
            Attachment plAttachment = plCard.ToAttachment();
            message.Attachments.Add(plAttachment);

            await context.PostAsync(message);
            context.Wait(HandleSuggestionResponse);
        }

        public async Task HandleSuggestionResponse(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            switch (message.Text.ToLower())
            {
                case "sports":
                    await ShowProgrammeList(context, message.Text.ToLower(), "1301,1302,1333,1322,1303");
                    break;
                case "movies":
                    await ShowProgrammeList(context, message.Text.ToLower(), "1409,1818,1808,1002,1823");
                    break;

                default:
                    var ccf = context.MakeMessage();
                    ccf.Text = "Let's try again.";
                    await context.PostAsync(ccf);
                    context.UserData.SetValue("welcomeMessageSeen", false);
                    context.UserData.SetValue("success", false);
                    context.Done("Failure");
                    break;
            }

        }

        private async Task ShowProgrammeList(IDialogContext context, string typeOfChannel, string channels)
        {
            var cc = context.MakeMessage();
            var suggestions = new SuggestionService().GetProgrammesOnChannels(channels);
            string text = "On the " + typeOfChannel + " channels now...";

            int count = 1;
            foreach (var suggestion in suggestions)
            {
                context.UserData.SetValue("sp" + count, suggestion.eventid);//.EventId);
                text += "\n1. _" + suggestion.title;//.Name;
                text += "_ on " + suggestion.channel.title;//.Channel; //+ " for the next ";// + (suggestion.EndTime - DateTime.Now).Minutes;
                count++;
            }
            text += "\n\nWould you like more details of any of these? Enter the number for more details.";
            cc.Text = text;
            await context.PostAsync(cc);

            context.Wait(HandleProgramSelection);
        }

        public async Task HandleProgramSelection(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

            var message = await argument;
            var cc = context.MakeMessage();
            string eventid;
            if (context.UserData.TryGetValue("sp" + message.Text, out eventid))
            {
                TVShow tvShow = new SuggestionService().GetById(eventid);
                cc.Type = "message";
                cc.Attachments = new List<Attachment>();

                var poster = new PosterService().Get(tvShow.title);
                var imgUrl = poster != null && poster.poster != null ? poster.poster : "https://cdn.instructables.com/FTU/1BBR/FLI8MT4O/FTU1BBRFLI8MT4O.MEDIUM.jpg";

                var plCard = new ThumbnailCard()
                {
                    Title = tvShow.title,
                    Subtitle = tvShow.channel.title + " (" + tvShow.channel.channelid + ") - " + tvShow.startAsDateTime.ToString(),
                    Text = $"{tvShow.shortDesc}",
                    Images = new List<CardImage> { new CardImage(url: imgUrl) },
                    Buttons = new List<CardAction> { new CardAction() { Value = "watch", Type = "postBack", Title = "Watch" } }
                };
                Attachment plAttachment = plCard.ToAttachment();
                cc.Attachments.Add(plAttachment);

                
                await context.PostAsync(cc);
                context.Wait(HandleProgramChosen);
            }
            else
            {
                await HandleSuggestionResponse(context, argument);
                await context.PostAsync(cc);
                context.UserData.SetValue("welcomeMessageSeen", false);
                context.UserData.SetValue("success", true);
                context.Done("Success");
            }



        }

        public async Task HandleProgramChosen(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            context.UserData.SetValue("welcomeMessageSeen", false);
            context.Done("Success");
        }

    }
}