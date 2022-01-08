using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using BookingBotRFT.Dialogs.Data;
using System.IO;

namespace BookingBotRFT.Dialogs
{
    public class BookingDialog: ComponentDialog
    {
        private  UserProfile _userProfile;
        public BookingDialog() : base(nameof(BookingDialog))
        {
            

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                NameStepAsync,
                EmailStepAsync,
                HotelStepStepAsync,
                TimeStepAsync,
                FinalizeStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            _userProfile = new UserProfile();

            stepContext.Values["profile"] = _userProfile;

            var promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please type in your name.")
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellation);
        }

        private async Task<DialogTurnResult> EmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            _userProfile = stepContext.Values["profile"] as UserProfile;
            _userProfile.Name = stepContext.Result as string;

            var promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please type in your email.")
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellation);
        }

        private async Task<DialogTurnResult> HotelStepStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            _userProfile = stepContext.Values["profile"] as UserProfile;

            _userProfile.Email = stepContext.Result as string;

            var promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please type in where you want to travel.")
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellation);
        }

        private async Task<DialogTurnResult> TimeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            _userProfile = stepContext.Values["profile"] as UserProfile;

            _userProfile.Hotel = stepContext.Result as string;

            var promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please type in when you want to travel.")
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellation);
        }

        private async Task<DialogTurnResult> FinalizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            _userProfile = stepContext.Values["profile"] as UserProfile;

            _userProfile.Date = stepContext.Result as string;

            Random r = new Random();

            string path = @"./Dialogs/Data/Clients.txt";

            StreamWriter wr = new StreamWriter(path);

            _userProfile.Id = r.Next(10000000, 99999999).ToString();

            wr.WriteLine($"{_userProfile.Id}, {_userProfile.Name}, {_userProfile.Hotel}, {_userProfile.Email}, {_userProfile.Date}");

            await stepContext.Context.SendActivityAsync("Thank you!");
            return await stepContext.EndDialogAsync();

        }
    }
}
