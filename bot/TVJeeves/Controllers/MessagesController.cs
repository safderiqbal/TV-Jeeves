using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using TVJeeves.Base.BusinessLogic;

namespace TVJeeves
{

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new EchoDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }



        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }


    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            await new SuggestionDialog().StartAsync(context);
        }
    }

    [Serializable]
    public class SuggestionDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var cc = context.MakeMessage();

            cc.Text = "Do any of these things sound good?";
            context.PostAsync(cc);
            
            var replyToConversation = context.MakeMessage();

            replyToConversation.Type = "message";
            replyToConversation.Attachments = new List<Attachment>();

            var suggestions = new SuggestionService().GetUserSuggestions();

            foreach (var suggestion in suggestions)
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

                HeroCard plCard = new HeroCard()
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
