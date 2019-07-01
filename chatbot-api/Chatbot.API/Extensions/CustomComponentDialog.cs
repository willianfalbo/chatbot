using System;
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

        // https://www.microsoftpressstore.com/articles/article.aspx?p=2854377&seqNum=3
        protected async Task SendTypingActivity(ITurnContext context, CancellationToken cancellationToken)
        {
            // this bot is only handling messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                var typingActivity = Activity.CreateTypingActivity();

                typingActivity.ReplyToId = context.Activity.Id;
                typingActivity.From = new ChannelAccount
                {
                    Id = context.Activity.Recipient.Id,
                    Name = context.Activity.Recipient.Name
                };
                typingActivity.Recipient = new ChannelAccount
                {
                    Id = context.Activity.From.Id,
                    Name = context.Activity.From.Name
                };
                typingActivity.Conversation = new ConversationAccount
                {
                    Id = context.Activity.Conversation.Id,
                    Name = context.Activity.Conversation.Name,
                    IsGroup = context.Activity.Conversation.IsGroup
                };

                // send typing activity
                await context.SendActivityAsync(typingActivity, cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}