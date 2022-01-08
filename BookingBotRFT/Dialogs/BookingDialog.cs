using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using BookingBotRFT.Dialogs.Data;

namespace BookingBotRFT.Dialogs
{
    public class BookingDialog: ComponentDialog
    {
        private readonly UserProfile _userprofile;
        public BookingDialog() : base(nameof(BookingDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {

                }));

            InitialDialogId = nameof(WaterfallDialog);
        }
    }
}
