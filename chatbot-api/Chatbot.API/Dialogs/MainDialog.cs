using System.Threading;
using System.Threading.Tasks;
using Chatbot.API.Extensions;
using Chatbot.Common.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : CustomComponentDialog
    {
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;
        private readonly ICompanyRegistryManager _companyRegistryManager;
        private readonly IAppSettings _appSettings;

        public MainDialog(IAppSettings appSettings, UserState userState, ConversationState conversationState, ICompanyRegistryManager companyRegistryManager)
            : base(nameof(MainDialog))
        {
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
            this._userState = userState ?? throw new System.ArgumentNullException(nameof(userState));
            this._conversationState = conversationState ?? throw new System.ArgumentNullException(nameof(conversationState));
            this._companyRegistryManager = companyRegistryManager ?? throw new System.ArgumentNullException(nameof(companyRegistryManager));

            AddDialog(new UserProfileDialog(_appSettings, _companyRegistryManager, _userState, _conversationState));

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
