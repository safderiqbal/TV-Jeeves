using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TVJeeves.Core.BusinessLogic;
using TVJeeves.Core.Entities;

namespace TVJeeves.Dialog
{
    [Serializable]
    public class TestDialog : IDialog<Channel>
    {
        protected IList<Channel> _channels { get; set; }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("What channel are you watching?");
            context.Wait(HandleMessageResponse);
        }

        public virtual async Task HandleMessageResponse(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            if (_channels != null)
                await HandleChannelSelection(context, argument);
            else
            {
                var message = await argument;

                _channels = new ChannelService().Get(message.Text);

                if (_channels.Count == 1)
                {
                    context.Done(_channels.First());
                    return;
                }

                var output = "Please confirm by selecting one of the options below\n";
                for (int i = 0; i < _channels.Count; i++)
                {
                    var chan = _channels.ElementAt(i);
                    output += $"{i + 1}. {chan.channelno} {chan.title}\n";
                }
                await context.PostAsync(output);
                context.Wait(HandleMessageResponse);
            }
        }

        public async Task HandleChannelSelection(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            int selectedOption;
            if(!int.TryParse(message.Text, out selectedOption))
            {
                await context.PostAsync("Sorry that was not an option");
                context.Wait(HandleMessageResponse);
            }
            var channel = _channels.ElementAt(selectedOption - 1);
            context.Done(channel);
        }
    }
}