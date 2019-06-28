using System.Threading;
using System.Threading.Tasks;
using Chatbot.API.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Chatbot.Common.Extensions;
using System.Linq;
using System;

namespace Microsoft.BotBuilderSamples
{
    // This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
    // to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
    // each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
    // The ConversationState is used by the Dialog system. The UserState isn't, however, it might have been used in a Dialog implementation,
    // and the requirement is that all BotState objects are saved at the end of a turn.
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly BotState _userState;
        protected readonly BotState _conversationState;
        protected readonly Dialog _dialog;
        protected readonly ILogger _logger;

        public DialogBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
        {
            _conversationState = conversationState;
            _userState = userState;
            _dialog = dialog;
            _logger = logger;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendTypingActivity(turnContext, cancellationToken);

            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            //TODO: wrap this up into the CustomComponentDialog
            var userStateAccessors = _userState.CreateProperty<UserPreference>(nameof(UserPreference));
            var profile = await userStateAccessors.GetAsync(turnContext, () => new UserPreference());

            string input = turnContext.Activity.Text?.Trim();
            // await turnContext.SendActivityAsync(MessageFactory.Text($"Response {input}."), cancellationToken);
            // await turnContext.SendActivityAsync(MessageFactory.Text($"UserHelpOption {profile.UserOption}."), cancellationToken);

            // check if user has given the expected option
            if (string.IsNullOrWhiteSpace(profile.UserOption) && !string.IsNullOrWhiteSpace(input))
            {
                var option = UserPreference.ChatbotOptions().FirstOrDefault(opt => opt.IsEqual(input));
                if (option != null)
                    profile.UserOption = option;
            }

            if (!string.IsNullOrWhiteSpace(profile.UserOption))
            {
                // Run the Dialog with the new message Activity.
                await _dialog.Run(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Eu preciso que você diga alguma das opções acima."), cancellationToken);
            }
        }

        private static async Task SendTypingActivity(ITurnContext context, CancellationToken cancellationToken)
        {
            // this bot is only handling messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // send typing activity
                //await context.SendActivityAsync(Activity.CreateTypingActivity(), cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
