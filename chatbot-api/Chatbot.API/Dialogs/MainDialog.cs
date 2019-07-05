using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Chatbot.API.Extensions;
using Chatbot.Common.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : CustomComponentDialog
    {
        private readonly IAppSettings _appSettings;
        private readonly IUserConversationManager _userConversationManager;
        private readonly IMapper _mapper;

        public MainDialog(
            IAppSettings appSettings,
            UserState userState,
            ConversationState conversationState,
            ICompanyRegistryManager companyRegistryManager,
            IUserConversationManager userConversationManager,
            IMapper mapper)
            : base(nameof(MainDialog), userState, conversationState)
        {
            if (userState is null)
                throw new System.ArgumentNullException(nameof(userState));
            if (conversationState is null)
                throw new System.ArgumentNullException(nameof(conversationState));
            if (companyRegistryManager is null)
                throw new System.ArgumentNullException(nameof(companyRegistryManager));
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
            this._userConversationManager = userConversationManager ?? throw new System.ArgumentNullException(nameof(userConversationManager));
            this._mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
            
            AddDialog(new UserProfileDialog(appSettings, companyRegistryManager, userState, conversationState, userConversationManager, mapper));

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
