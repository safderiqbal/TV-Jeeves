using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using TVJeeves.Core.BusinessLogic;
using TVJeeves.Core.Entities;

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
                        if (!string.IsNullOrEmpty(await response))
                        {
                            var res = await response;
                            var channels = new ChannelService().Get(res);

                            if (!channels.Any())
                                return Chain.Return(new Tuple<string, List<Channel>>(res, new List<Channel>()));

                            context.UserData.SetValue("channels", channels);
                            context.UserData.SetValue("channelsInd", 0);
                            return Chain.Return(new Tuple<string, List<Channel>>(res, channels));
                        }
                        return Chain.Return(new Tuple<string, List<Channel>>("Error", new List<Channel>()));
                    })
        //            .ContinueWith(async (context, response) =>
        //            {
        //                var r = await response;
        //                var channels = context.UserData.Get<List<Channel>>("channels");
        //                var currentInd = context.UserData.Get<int>("channelsInd");
        //                var channel = channels.ElementAt(currentInd);
        //                return Chain.From(() => new PromptDialog.PromptString($"is this you channel? {channel.title}",
        //"Didn't get that!", 5))
        //                    .ContinueWith(async (cont, resp) =>
        //                    {
        //                        var x = await resp;
        //                        if (x.ToLower() == "Yes")
        //                        {
        //                            context.UserData.SetValue("watching", channel);
        //                            return Chain.Return("yes it is");
        //                        }

        //                        currentInd++;
        //                        channel = channels.ElementAt(currentInd);
        //                        return Chain.Return("no it isnt");
        //                    });
        //            })
        .ContinueWith(async (context, response) =>
        {
            var channel = (Tuple<string, List<Channel>>)await response;
            Channel output;

            if (!channel.Item2.Any())
                return Chain.Return($"Cannot find channel {channel.Item1}");

            foreach (var chan in channel.Item2)
            {
                //return new PromptDialog.PromptConfirm($"are you watching {chan.title}", "", 3);
            }

            return Chain.Return($"you are watching {channel.Item1}");
        })
                    .ContinueWith(async (context, response) =>
                     {
                         await response;
                         var chan = context.UserData.Get<Channel>("watching");
                         return Chain.Return($"you want to watch {chan.title} and {chan.channelid} and {chan.channelno}");
                     });
                }),
                new DefaultCase<IMessageActivity, IDialog<string>>((ctx, msg) =>
                {
                    return Chain.Return("Sorry, I didn't understand that.");
                })).Unwrap().PostToUser();
    }
}