using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Chatbot.API.DTO;
using Chatbot.Common.Helpers;
using Chatbot.Common.Interfaces;
using Chatbot.Model.Manager;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Chatbot.API.Helpers
{
    public class DialogHelper : IDialogHelper
    {
        public UserState _userState { get; }
        public ConversationState _conversationState { get; }
        public IAppSettings _appSettings { get; }
        public IMapper _mapper { get; }
        public IUserConversationManager _userConversationManager { get; }

        public DialogHelper(
            UserState userState,
            ConversationState conversationState,
            IAppSettings appSettings,
            IMapper mapper,
            IUserConversationManager userConversationManager)
        {
            this._userState = userState ?? throw new System.ArgumentNullException(nameof(userState));
            this._conversationState = conversationState ?? throw new System.ArgumentNullException(nameof(conversationState));
            this._appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._userConversationManager = userConversationManager ?? throw new ArgumentNullException(nameof(userConversationManager));
        }

        public Attachment CreateAdaptiveCardAttachment(string filePath, object data = null)
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

        // public async Task<T> GetUserState<T>(ITurnContext context, CancellationToken cancellationToken) where T : new()
        // {
        //     var stateAccessor = this._userState.CreateProperty<T>(typeof(T).Name);
        //     var content = await stateAccessor.GetAsync(context, () => new T(), cancellationToken);
        //     return content;
        // }

        // public async Task SetUserState<T>(ITurnContext context, T content, CancellationToken cancellationToken)
        // {
        //     var stateAccessor = this._userState.CreateProperty<T>(typeof(T).Name);
        //     await stateAccessor.SetAsync(context, content, cancellationToken);
        // }

        public async Task<T> GetConversationState<T>(ITurnContext context, CancellationToken cancellationToken) where T : new()
        {
            var stateAccessor = this._conversationState.CreateProperty<T>(typeof(T).Name);
            var content = await stateAccessor.GetAsync(context, () => new T(), cancellationToken);
            return content;
        }

        public async Task SetConversationState<T>(ITurnContext context, T content, CancellationToken cancellationToken)
        {
            var stateAccessor = this._conversationState.CreateProperty<T>(typeof(T).Name);
            await stateAccessor.SetAsync(context, content, cancellationToken);
        }

        // https://www.microsoftpressstore.com/articles/article.aspx?p=2854377&seqNum=3
        public async Task SendTypingActivity(ITurnContext context, CancellationToken cancellationToken)
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

        public async Task SaveConversationDB(ITurnContext context, CancellationToken cancellationToken)
        {
            var conversation = await this.GetConversationState<UserConversationDTO>(context, cancellationToken);

            await this._userConversationManager.SaveAsync(this._mapper.Map<UserConversation>(conversation));
        }
    }
}