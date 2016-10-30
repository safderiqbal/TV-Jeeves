using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TVJeeves.Base.BusinessLogic;
using TVJeeves.Core.BusinessLogic;
using TVJeeves.Core.Entities;

namespace TVJeeves.Dialog
{
    [Serializable]
    public class TopFiveDialog : IDialog<object>
    {
        private Tuple<string, List<Channel>> _things { get; set; }
        public TopFiveDialog(Tuple<string, List<Channel>> things)
        {
            _things = things;
        }

        public async Task StartAsync(IDialogContext context)
        {
            var cc = context.MakeMessage();
            cc.Type = "message";
            cc.Attachments = new List<Attachment>();

            var genre = _things.Item2.First().genre.First();
            var currentlyOnChannel = new SuggestionService().Get(_things.Item2.First().channelid.ToString()).First();

            var shows = new GenreService().Get(genre.genreid, currentlyOnChannel.subgenre.ToString()).Where(x => x.scheduleStatus != "PLAYING_NOW").ToList();

            var output = $"You are currently watching **{currentlyOnChannel.title}**\n";
            output += $"Here are some other programs currently showing of the same genre of **{genre.name}** \n";

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
                        Subtitle = tvShow.channel.title + " (" + tvShow.channel.channelid + ") - " + tvShow.startAsDateTime.ToString(),
                        Text = $"{tvShow.shortDesc} - {tvShow.startAsDateTime.ToString()}",
                        Images = new List<CardImage> { new CardImage(url: imgUrl) }
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    cc.Attachments.Add(plAttachment);

                    //output += $"{shows[i].channel.channelno}. {shows[i].channel.title} {shows[i].title} \n";
                    //output += $"**Short Desc** {shows[i].shortDesc} *{shows[i].startAsDateTime}*\n";
                }
            } else
            {
                output += "*Sorry no matches found.";
            }

            cc.Text = output;

            await context.PostAsync(cc);
            //context.Reset();
            context.Wait(HandleSuggestionResponse);
        }

        public async Task HandleSuggestionResponse(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            await argument;
            context.Done(this);
        }
    }
}