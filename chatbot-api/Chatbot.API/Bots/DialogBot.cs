using System.Threading;
using System.Threading.Tasks;
using Chatbot.API.DTO;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Chatbot.Common.Extensions;
using System.Linq;

namespace Microsoft.BotBuilderSamples
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly BotState _userState;
        protected readonly BotState _conversationState;
        protected readonly Dialog _dialog;
        protected readonly ILogger _logger;

        public DialogBot(
            ConversationState conversationState, 
            UserState userState, 
            T dialog, 
            ILogger<DialogBot<T>> logger)
        {
            _conversationState = conversationState;
            _userState = userState;
            _dialog = dialog;
            _logger = logger;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // await SendTypingActivity(turnContext, cancellationToken);

            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            //TODO: wrap this up into the CustomComponentDialog
            var userStateAccessors = _userState.CreateProperty<UserPreferenceDTO>(nameof(UserPreferenceDTO));
            var userPreference = await userStateAccessors.GetAsync(turnContext, () => new UserPreferenceDTO());

            string input = turnContext.Activity.Text?.Trim();
            // await turnContext.SendActivityAsync(MessageFactory.Text($"Response {input}."), cancellationToken);

            // check if user has given the expected option
            if (string.IsNullOrWhiteSpace(userPreference.UserOption) && !string.IsNullOrWhiteSpace(input))
            {
                var option = UserPreferenceDTO.ChatbotOptions().FirstOrDefault(opt => opt.IsEqual(input));
                if (option != null)
                    userPreference.UserOption = option;
            }

            if (!string.IsNullOrWhiteSpace(userPreference.UserOption))
            {
                // Run the Dialog with the new message Activity.
                await _dialog.Run(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Eu espero alguma das opções acima."), cancellationToken);
            }
        }

    }
}
