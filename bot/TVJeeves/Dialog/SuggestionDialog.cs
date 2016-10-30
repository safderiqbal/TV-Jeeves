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
    public class SuggestionDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var cc = context.MakeMessage();

            cc.Text = "Are you looking for something to watch?";
            await context.PostAsync(cc);

            cc.Text = "Do any of these things sound good?";
            await context.PostAsync(cc);

            var replyToConversation = context.MakeMessage();

            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();

            var sportSuggestions = new SuggestionService().GetUserSuggestions();

            foreach (var suggestion in sportSuggestions)
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: suggestion.ImageUrl));

                List<CardAction> cardButtons = new List<CardAction>();
                CardAction plButton = new CardAction()
                {
                    Value = "watch",
                    Type = "postBack",
                    Title = "Watch"
                };
                cardButtons.Add(plButton);

                ThumbnailCard plCard = new ThumbnailCard()
                {
                    Title = suggestion.Name,
                    Subtitle = suggestion.Channel,
                    Images = cardImages,
                    Buttons = cardButtons
                };
                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
            }


            replyToConversation.AttachmentLayout = "carousel";

            await context.PostAsync(replyToConversation);
            context.Wait(HandleSuggestionResponse);
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