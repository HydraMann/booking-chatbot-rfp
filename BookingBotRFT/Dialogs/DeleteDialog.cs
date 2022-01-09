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
    public class DeleteDialog: ComponentDialog
    {
        public DeleteDialog() : base(nameof(DeleteDialog))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IdStepAsync,
                ConfirmStepAsync,
                FinalizeStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IdStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please give the booking id")
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellation);

        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            stepContext.Values["id"] = stepContext.Result as string;

            var promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Are you sure?")
            };

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), promptOptions, cancellation);


        }


        private async Task<DialogTurnResult> FinalizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var result = (bool)stepContext.Result;

            if (result)
            {
                string path = @"./Dialogs/Data/Clients.txt";

                StreamReader sr = new StreamReader(path);

                List<UserProfile> profiles = new List<UserProfile>();


                while (sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line != null && line != string.Empty)
                    {
                        string[] seperated = line.Split(',');
                        UserProfile profile = new UserProfile(seperated[0], seperated[1], seperated[2], seperated[3], seperated[4]);

                        if (profile.Id.Contains((string)stepContext.Values["id"]))
                        {
                            continue;
                        }

                        profiles.Add(profile);
                    }
                }

                sr.Close();

                StreamWriter wr = new StreamWriter(path);

                for (int i = 0; i < profiles.Count; i++)
                {
                    wr.WriteLine($"{profiles[i].Id},{profiles[i].Name},{profiles[i].Email},{profiles[i].Hotel},{profiles[i].Date}");
                }

                wr.Close();

                await stepContext.Context.SendActivityAsync("Your data has been deleted.");

            }

            return await stepContext.EndDialogAsync();
        }


    }
}
