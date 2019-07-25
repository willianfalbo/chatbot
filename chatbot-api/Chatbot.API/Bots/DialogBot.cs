using System.Threading;
using System.Threading.Tasks;
using Chatbot.API.DTO;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Chatbot.Common.Extensions;
using System.Linq;
using Chatbot.API.Helpers;
using System.IO;
using System.Net;

namespace Chatbot.API.Bots
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly Dialog _dialog;
        protected readonly ILogger _logger;
        protected readonly IDialogHelper _helper;

        public DialogBot(
            IDialogHelper dialogHelper,
            T dialog,
            ILogger<DialogBot<T>> logger)
        {
            _helper = dialogHelper ?? throw new System.ArgumentNullException(nameof(dialogHelper));
            _dialog = dialog;
            _logger = logger;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _helper.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _helper.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            // We know the user is sending an attachment as there is at least one item
            // in the Attachments list.
            if (turnContext?.Activity?.Attachments != null && turnContext.Activity.Attachments.Any())
                await HandleAttachment(turnContext, cancellationToken);
            else
                await HandleMessage(turnContext, cancellationToken);
        }

        // Handle attachments uploaded by users. The bot receives an <see cref="Attachment"/> in an <see cref="Activity"/>.
        // The activity has a "IList{T}" of attachments.    
        // Not all channels allow users to upload files. Some channels have restrictions
        // on file type, size, and other attributes. Consult the documentation for the channel for
        // more information. For example Skype's limits are here
        // <see ref="https://support.skype.com/en/faq/FA34644/skype-file-sharing-file-types-size-and-time-limits"/>.
        private async Task HandleAttachment(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var file in turnContext.Activity.Attachments)
            {
                // Determine where the file is hosted.
                var remoteFileUrl = file.ContentUrl;

                // Save the attachment to the system temp directory.
                var localFileName = Path.Combine(Path.GetTempPath(), file.Name);

                // Download the actual attachment
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(remoteFileUrl, localFileName);
                }

                await turnContext.SendActivityAsync(MessageFactory.Text($"O anexo {file.Name} foi salvo!"), cancellationToken);
            }
        }

        private async Task HandleMessage(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            var conversation = await _helper.UserAccessor.GetAsync(turnContext, () => new UserConversationDTO());

            string input = turnContext.Activity.Text?.Trim();
            // await turnContext.SendActivityAsync(MessageFactory.Text($"Response {input}."), cancellationToken);

            // check if user has given the expected option
            if (string.IsNullOrWhiteSpace(conversation?.UserPreference?.UserOption) && !string.IsNullOrWhiteSpace(input))
            {
                var option = UserPreferenceDTO.ChatbotOptions().FirstOrDefault(opt => opt.IsEqual(input));
                if (option != null)
                    conversation.UserPreference.UserOption = option;
            }

            if (!string.IsNullOrWhiteSpace(conversation?.UserPreference?.UserOption))
            {
                // Run the Dialog with the new message Activity.
                await _dialog.Run(turnContext, _helper.ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Eu espero alguma das opções acima."), cancellationToken);
            }
        }

    }
}
