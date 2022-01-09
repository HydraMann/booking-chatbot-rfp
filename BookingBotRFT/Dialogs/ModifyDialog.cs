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
    public class ModifyDialog: ComponentDialog
    {
        private UserProfile _userProfile;
        public ModifyDialog() : base(nameof(ModifyDialog))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IdStepAsync,
                ChooseStepAsync,
                ModifyStepAsync,
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

        private async Task<DialogTurnResult> ChooseStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            stepContext.Values["id"] = stepContext.Result as string;

            var promptOptions = new PromptOptions()
            {
                Prompt = (Activity)MessageFactory.Attachment(CardFactory("What do you wish to change?"))
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellation);
        }

        private async Task<DialogTurnResult> ModifyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            stepContext.Values["change"] = stepContext.Result as string;

            var promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text("What is the new value?")
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellation);

        }

        private async Task<DialogTurnResult> FinalizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var result = stepContext.Result as string;

            string path = @"./Dialogs/Data/Clients.txt";

            StreamReader sr = new StreamReader(path);

            List<UserProfile> profiles = new List<UserProfile>();

            int index = 0;
            int current = 0;

            try
            {
                while (sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line != null && line != string.Empty)
                    {
                        string[] seperated = line.Split(',');
                        UserProfile profile = new UserProfile(seperated[0], seperated[1], seperated[2], seperated[3], seperated[4]);

                        if (profile.Id.Contains((string)stepContext.Values["id"]))
                        {
                            index = current;
                        }

                        current++;

                        profiles.Add(profile);
                    }
                }

                sr.Close();

                switch ((string)stepContext.Values["change"])
                {
                    case "Name":
                        profiles[index].Name = stepContext.Values["change"].ToString();
                        break;

                    case "Email":
                        profiles[index].Email = stepContext.Values["change"].ToString();
                        break;

                    case "Hotel":
                        profiles[index].Hotel = stepContext.Values["change"].ToString();
                        break;

                    case "Date":
                        profiles[index].Date = stepContext.Values["change"].ToString();
                        break;

                    default:
                        await stepContext.Context.SendActivityAsync("No value found");
                        break;
                }

                StreamWriter wr = new StreamWriter(path);

                for (int i = 0; i < profiles.Count; i++)
                {
                    wr.WriteLine($"{profiles[i].Id},{profiles[i].Name},{profiles[i].Email},{profiles[i].Hotel},{profiles[i].Date}");
                }

                wr.Close();

                await stepContext.Context.SendActivityAsync("Your data has been changed.");
            }
            catch (Exception)
            {

            }

            return await stepContext.EndDialogAsync();
        }

        private Attachment CardFactory(string prompt)
        {
            var card = new HeroCard
            {
                Text = prompt,
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, title: "Name", value: "Name"),
                    new CardAction(ActionTypes.ImBack, title: "Email", value: "Email"),
                    new CardAction(ActionTypes.ImBack, title: "Hotel", value: "Hotel"),
                    new CardAction(ActionTypes.ImBack, title: "Date", value: "Date"),
                },
            };

            var reply = card.ToAttachment();

            return reply;
        }
    }
}
