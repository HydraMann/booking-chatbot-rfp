using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

using BookingBotRFT.Dialogs.Data;
using BookingBotRFT.Dialogs;

namespace BookingBotRFT
{
    
    public class MainDialog: ComponentDialog
    {

        public MainDialog() : base(nameof(MainDialog))
        {

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new BookingDialog());
            AddDialog(new ModifyDialog());
            AddDialog(new DeleteDialog());
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            var waterfallSteps = new WaterfallStep[]
            {
                MessageStepAsync,
                ActStepAsync,
                ConfirmStepAsync,
                FinalizeStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> MessageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var promptOptions = new PromptOptions()
            {
                Prompt = (Activity)MessageFactory.Attachment(CardFactory("Hello User! What do you wish to do?"))
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellation);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var result = stepContext.Result as string;

            switch (result)
            {
                case "Booking":
                    return await stepContext.BeginDialogAsync(nameof(BookingDialog),null, cancellation);

                case "Modify":
                    return await stepContext.BeginDialogAsync(nameof(ModifyDialog), null, cancellation);

                case "Delete":
                    return await stepContext.BeginDialogAsync(nameof(DeleteDialog), null, cancellation);

                default:
                    return await stepContext.NextAsync();
            }


        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions() { Prompt = MessageFactory.Text("Is there anything else I can do for you?") }, cancellation);
        }

        private async Task<DialogTurnResult> FinalizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var result = (bool)stepContext.Result;

            if (result)
            {
                return await stepContext.ReplaceDialogAsync(Id);
            }

            await stepContext.Context.SendActivityAsync("Thank you!");

            return await stepContext.EndDialogAsync();
        }

        private Attachment CardFactory(string prompt)
        {
            var card = new HeroCard
            {
                Text = prompt,
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, title: "Booking", value: "Booking"),
                    new CardAction(ActionTypes.ImBack, title: "Delete", value: "Delete"),
                    new CardAction(ActionTypes.ImBack, title: "Modify", value: "Modify"),
                },
            };

            var reply = card.ToAttachment();

            return reply;
        }

    }
}
