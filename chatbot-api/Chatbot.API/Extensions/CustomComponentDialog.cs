using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Chatbot.Common.Handlers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Chatbot.API.Extensions
{
    public class CustomComponentDialog : ComponentDialog
    {
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;

        public CustomComponentDialog(string dialogId, UserState userState, ConversationState conversationState)
            : base(dialogId)
        {
            this._userState = userState ?? throw new System.ArgumentNullException(nameof(userState));
            this._conversationState = conversationState ?? throw new System.ArgumentNullException(nameof(conversationState));
        }

        protected Attachment CreateAdaptiveCardAttachment(string filePath, object data = null)
        {
            var adaptiveCardJson = string.Empty;

            if (data != null)
                adaptiveCardJson = TemplateHandler.RenderFile(filePath, data);
            else
                adaptiveCardJson = File.ReadAllText(filePath);

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };

            return adaptiveCardAttachment;
        }

        protected async Task<T> GetUserState<T>(ITurnContext context) where T : new()
        {
            var stateAccessor = _userState.CreateProperty<T>(typeof(T).Name);
            var content = await stateAccessor.GetAsync(context, () => new T());
            return content;
        }

        protected async Task SetUserState<T>(ITurnContext context, T content, CancellationToken cancellationToken)
        {
            var stateAccessor = _userState.CreateProperty<T>(typeof(T).Name);
            await stateAccessor.SetAsync(context, content, cancellationToken);
        }

        protected async Task<T> GetConversationState<T>(ITurnContext context) where T : new()
        {
            var stateAccessor = _conversationState.CreateProperty<T>(typeof(T).Name);
            var content = await stateAccessor.GetAsync(context, () => new T());
            return content;
        }

        protected async Task SetConversationState<T>(ITurnContext context, T content, CancellationToken cancellationToken)
        {
            var stateAccessor = _conversationState.CreateProperty<T>(typeof(T).Name);
            await stateAccessor.SetAsync(context, content, cancellationToken);
        }
    }
}