using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
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
    public class WatchLuisDialog : LuisDialog<Watch>
    {
        private readonly BuildFormDelegate<Watch> WatchForm;

        internal WatchLuisDialog(BuildFormDelegate<Watch> watchForm)
        {
            WatchForm = watchForm;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Pardon. I didn't quite understand you.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("ChannelSurfing")]
        public async Task WatchNextForm(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);
            var form = new FormDialog<Watch>(new Watch(), WatchForm, FormOptions.PromptInStart, entities);
            var dialog = new TestDialog();
            context.Call(dialog, PromptComplete);
        }

        private async Task PromptComplete(IDialogContext context, IAwaitable<Channel> result) {
            var channel = await result;
            await context.PostAsync($"test complete, great channel choice {channel.channelno}:{channel.title}");

            PromptDialog.Confirm(context, OnAcceptance, "Do you want to watching something similar?");
        }

        private async Task OnAcceptance(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
                await context.PostAsync("Let see what we have...");
            else
                await context.PostAsync("What genre would you like to watch?");

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

        private async Task FormComplete(IDialogContext context, IAwaitable<Watch> result)
        {
            Watch order = null;
            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the form!");
                return;
            }

            var channels = new ChannelService().Get(order.channel);
            var show = new SuggestionService().Get(channels.First().channelid.ToString()).First();

            await context.PostAsync($"Channel {order.channel} is currently showing {show.title}");
            await context.PostAsync($"Please wait while we find some suggestions.");

            var shows = new GenreService().Get(show.genre.ToString(), show.subgenre.ToString()).Where(x => x.scheduleStatus != "PLAYING_NOW").ToList();

            var cc = context.MakeMessage();
            cc.Type = "message";
            cc.Attachments = new List<Attachment>();

            if (shows.Count != 0)
            {
                for (int i = 0; i < (shows.Count >= 10 ? 10 : shows.Count); i++)
                {
                    var tvShow = shows[i];

                    var poster = new PosterService().Get(tvShow.title);
                    var imgUrl = poster != null && poster.poster != null ? poster.poster : "https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcRj0YzDMrnC8kqGjvTH3tQ_VpVY4HbtcpGCNcJ_tR4WdiMKvjYc";

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
            }
            else
            {
                cc.Text = "*Sorry no matches found.";
            }

            await context.PostAsync(cc);

            context.Wait(MessageReceived);
        }
    }

    [Serializable]
    public class Watch
    {
        [Prompt("What channel are you watching? {||}")]
        public string channel;
    }
    public enum ChannelOptions
    {
        Channel1, Channel2, Channel3
    }
}