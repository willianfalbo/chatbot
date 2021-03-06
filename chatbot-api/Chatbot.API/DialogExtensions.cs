﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Chatbot.Api
{
    public static class DialogExtensions
    {
        public static async Task Run(this Dialog dialog, ITurnContext turnContext, IStatePropertyAccessor<DialogState> accessor, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dialogSet = new DialogSet(accessor);
            dialogSet.Add(dialog);

            var dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken);

            // TODO: Remove this if clause entirely when microsoft releases the AdaptiveCardPrompt
            // https://github.com/microsoft/botframework-sdk/issues/5396
            // this is a workaround for using AdaptiveCardsPrompt
            // https://stackoverflow.com/questions/56180596/i-am-using-bot-framework-v4-3-i-want-to-retrieve-adaptive-card-submit-values
            if (string.IsNullOrWhiteSpace(dialogContext.Context.Activity.Text))
            {
                dialogContext.Context.Activity.Text = dialogContext.Context.Activity.Value.ToString();
            }

            var results = await dialogContext.ContinueDialogAsync(cancellationToken);
            if (results.Status == DialogTurnStatus.Empty)
            {
                await dialogContext.BeginDialogAsync(dialog.Id, null, cancellationToken);
            }
        }
    }
}
