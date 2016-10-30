using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TVJeeves.Base.BusinessLogic;
using TVJeeves.Base.Entities;

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
                    var cc = context.MakeMessage();
                    var suggestions = new SuggestionService().GetProgrammesOnChannels("1301,1302,1333,1322,1303");
                    string text = "On the sports channels now...";

                    int count = 1;
                    foreach (var suggestion in suggestions)
                    {
                        context.UserData.SetValue("sp" + count, suggestion.EventId);
                        text += "\n1. _" + suggestion.Name;
                        text += "_ on " + suggestion.Channel; //+ " for the next ";// + (suggestion.EndTime - DateTime.Now).Minutes;
                        count++;
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
                    string eventid;
                    context.UserData.TryGetValue("sp" + message.Text, out eventid);
                    Suggestion s = new SuggestionService().GetById(eventid);
                    ccf.Text = "That works : " + s.Name;
                    await context.PostAsync(ccf);
                    break;
            }

        }
        public async Task HandleProgramSelection(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            
            var message = await argument;
            var ccf = context.MakeMessage();
            string eventid;
            context.UserData.TryGetValue("sp" + message.Text, out eventid);
            Suggestion s = new SuggestionService().GetById(eventid);
            ccf.Text = "That works : " + s.Name;

            await context.PostAsync(ccf);
            context.Done("stes");
        }

    }
}