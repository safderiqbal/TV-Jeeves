using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TVJeeves.Base.BusinessLogic;
using TVJeeves.Core.BusinessLogic;
using TVJeeves.Core.Entities;

namespace TVJeeves.Dialog
{
    [LuisModel("d06caa29-6134-4bd8-ae11-860d3e329df2", "d015704f441847dcba2dfedeeff6669c")]
    [Serializable]
    public class WatchLuisDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Pardon. I didn't quite understand you.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("ChannelSurfing")]
        public async Task WatchNextForm(IDialogContext context, LuisResult result)
        {
            var dialog = new TestDialog();
            context.Call(dialog, PromptComplete);
        }

        private async Task PromptComplete(IDialogContext context, IAwaitable<Channel> result) {
            var channel = await result;

            context.UserData.SetValue("SelectedChannel", channel);
            await context.PostAsync($"great channel choice {channel.channelno}:{channel.title}");

            PromptDialog.Confirm(context, OnAcceptance, "Do you want to watching something similar?");
        }

        private async Task OnAcceptance(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                var selectedChannel = context.UserData.Get<Channel>("SelectedChannel");
                var currentShow = new SuggestionService().Get(selectedChannel.channelid.ToString()).First();
                context.Call(new ChannelFinderDialog(currentShow), FoundTvShows);
            }
            else
                await context.PostAsync("What genre would you like to watch?");
        }

        private async Task FoundTvShows(IDialogContext context, IAwaitable<List<TVShow>> result)
        {
            var tvshows = await result;

            var cc = context.MakeMessage();
            cc.Type = "message";
            cc.Attachments = new List<Attachment>();

            var output = "I think you will find the following programmes absolutely riveting \n";

            if (tvshows.Count != 0)
            {
                foreach (var tvShow in tvshows.Take(10))
                {
                    var poster = new PosterService().Get(tvShow.title);
                    var imgUrl = poster?.poster ?? "https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcRj0YzDMrnC8kqGjvTH3tQ_VpVY4HbtcpGCNcJ_tR4WdiMKvjYc";

                    var plCard = new ThumbnailCard()
                    {
                        Title = tvShow.title,
                        Subtitle = tvShow.channel.title + " (" + tvShow.channel.channelno + ") - " + tvShow.startAsDateTime.ToString(),
                        Text = $"{tvShow.shortDesc} - {tvShow.startAsDateTime.ToString()}",
                        Images = new List<CardImage> { new CardImage(url: imgUrl) }
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    cc.Attachments.Add(plAttachment);
                }

                cc.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            } else
            {
                output += "*Sorry no matches found.";
            }

            cc.Text = output;

            await context.PostAsync(cc);

            await context.PostAsync("I hope these treats are to your taste!");

            context.Wait(MessageReceived);
        }

        [LuisIntent("procrastination")]
        public async Task ReplyToProcrastination(IDialogContext context, LuisResult result)
        {
            var list = new List<string>
            {
                "cad","bounder", "punk","counter-jumper","oik",
                "nincompoops", "lickspittle", "twerp", "milksop",
                "whippersnapper", "blighter", "clot", "right charlie", "'erbert", "rapscallion",
                "leathern jerkin", "crystal-button", "not-pated", "agate-ring", "puke-stocking", "caddis-garter", "smooth-tongue", "Spanish pouch"
            };
            await context.PostAsync($"You sir are a {list.ElementAt(new Random().Next(0, list.Count))}, Goodbye!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi, My name is Jeeves. How may I help you?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Parting")]
        public async Task Parting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Goodbye, I hope my help was pleasing.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("CheckChannel")]
        public async Task CheckChannel(IDialogContext context, LuisResult result)
        {
            var foundEntity = result.Entities.FirstOrDefault();

            if (foundEntity != null)
            {
                var channels = new ChannelService().Get(foundEntity.Entity);
                if (!channels.Any())
                {
                    await context.PostAsync($"Sorry. I couldn't find channel {foundEntity.Entity}. Please try again.");
                    context.Wait(MessageReceived);
                    return;
                }

                var channel = channels.First();
                var currentShow = new SuggestionService().Get(channel.channelid.ToString()).First();
                await context.PostAsync($"{currentShow.channel.title} is currently showing {currentShow.title}");
            }
            else
            {
                await context.PostAsync("Pardon. I couldn't understand which channel you are watching.");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Suggestion")]
        public async Task Suggestion(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Here are some of the shows that everyone is talking about?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Favourite")]
        public async Task Favourite(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I've selected some of your favourite channels, anything here here interest you?");
            context.Wait(MessageReceived);
        }
    }
}