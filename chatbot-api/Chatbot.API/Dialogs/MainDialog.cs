﻿using System.Threading;
using System.Threading.Tasks;
using Chatbot.Api.Extensions;
using Chatbot.Api.Helpers;
using Chatbot.Common.Interfaces;
using Microsoft.Bot.Builder.Dialogs;

namespace Chatbot.Api.Dialogs
{
    public class MainDialog : CustomComponentDialog
    {
        private readonly IDialogHelper _helper;

        public MainDialog(
            IDialogHelper helper,
            ICompanyRegistryManager companyRegistryManager)
            : base(nameof(MainDialog))
        {
            this._helper = helper ?? throw new System.ArgumentNullException(nameof(helper));

            AddDialog(new UserProfileDialog(helper, companyRegistryManager ?? throw new System.ArgumentNullException(nameof(companyRegistryManager))));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                StartUserProfileDialogStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> StartUserProfileDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // begin the first dialog
            return await stepContext.BeginDialogAsync(nameof(UserProfileDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // var userInfo = (UserProfile)stepContext.Result;

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
