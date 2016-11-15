using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using TVJeeves.Core.BusinessLogic;
using TVJeeves.Core.Entities;

namespace TVJeeves.Dialog
{
    public class ChannelFinderDialog : IDialog<List<TVShow>>
    {
        private readonly TVShow _currentShow;

        public ChannelFinderDialog(TVShow currentShow)
        {
            _currentShow = currentShow;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Let see what we have...");

            var shows = new GenreService().Get(_currentShow.genre.ToString(), _currentShow.subgenre.ToString()).Where(x => x.scheduleStatus != "PLAYING_NOW").ToList();

            context.Done(shows);
        }
    }
}