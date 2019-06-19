using System.Threading;
using System.Threading.Tasks;
using Chatbot.API.Extensions;
using Chatbot.Common.Interfaces;
using Chatbot.Model.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : CustomComponentDialog
    {
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;
        private readonly ICompanyRegistryManager _companyRegistryManager;
        private readonly IConfiguration _configuration;

        public MainDialog(UserState userState, ICompanyRegistryManager companyRegistryManager, IConfiguration configuration, ConversationState conversationState)
            : base(nameof(MainDialog))
        {
            this._userState = userState ?? throw new System.ArgumentNullException(nameof(userState));
            this._conversationState = conversationState ?? throw new System.ArgumentNullException(nameof(conversationState));
            this._companyRegistryManager = companyRegistryManager ?? throw new System.ArgumentNullException(nameof(companyRegistryManager));
            this._configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));

            AddDialog(new UserProfileDialog(_configuration, _companyRegistryManager, _userState, _conversationState));

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
