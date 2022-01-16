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

namespace BookingBotRFT.Dialogs
{
    public class ContactSupportDialog : ComponentDialog
    {
        public ContactSupportDialog() : base(nameof(ContactSupportDialog))
        {


            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
               MessegeStepAsync,
               ConfirmStepAsync,
               FinalizeStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        
        private async Task<DialogTurnResult> MessegeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please type in your message")
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellation);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            stepContext.Values["message"] = stepContext.Result as string;
            var promptOptions = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Do you want to create a ticket?")
            };

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), promptOptions, cancellation);
        }

        private async Task<DialogTurnResult> FinalizeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellation)
        {
            var result = (bool)stepContext.Result;
            if (result)
            {
                await stepContext.Context.SendActivityAsync("Ticket created");
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Process cancaled");
            }
            return await stepContext.EndDialogAsync();
        }
    }
}
