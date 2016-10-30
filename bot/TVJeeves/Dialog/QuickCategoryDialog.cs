using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TVJeeves.Base.BusinessLogic;

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
                Title = "Do you want a quick look at what's on?",
                Subtitle = "",
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
                    var cc = context.MakeMessage();
                    var suggestions = new SuggestionService().GetProgrammesOnChannels(new List<int>());
                    string text = "On the sports channels now...";
                    foreach (var suggestion in suggestions)
                    {
                        text += "\n1. " + suggestion.Name;
                        text += " on " + suggestion.Channel + " for the next ";// + (suggestion.EndTime - DateTime.Now).Minutes;
                    }
                    text += "\n\nWould you like more details of any of these? Enter the number for more details.";
                    cc.Text = text;
                    await context.PostAsync(cc);

                    context.Wait(HandleProgramSelection);
                    break;
                case "movies":

                    break;

                default:
                    var ccf = context.MakeMessage();

                    ccf.Text = "That works : " + message.Text;
                    await context.PostAsync(ccf);
                    break;
            }

        }
        public async Task HandleProgramSelection(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            
            var message = await argument;
            var ccf = context.MakeMessage();

            ccf.Text = "That works : " + message.Text;
            await context.PostAsync(ccf);
            context.Done("stes");
        }

    }
}