using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Chatbot.Api.DTO;
using Chatbot.Common.Configuration;
using Chatbot.Common.Helpers;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Chatbot.Api.Helpers
{
    public class DialogHelper : IDialogHelper
    {
        public UserState UserState { get; }
        public ConversationState ConversationState { get; }
        public IStatePropertyAccessor<UserConversationDTO> UserAccessor { get; }
        public AppSettings AppSettings { get; }
        public IMapper Mapper { get; }

        public DialogHelper(
            UserState userState,
            ConversationState conversationState,
            AppSettings appSettings,
            IMapper mapper)
        {
            this.UserState = userState ?? throw new System.ArgumentNullException(nameof(userState));
            this.ConversationState = conversationState ?? throw new System.ArgumentNullException(nameof(conversationState));
            this.UserAccessor = UserState.CreateProperty<UserConversationDTO>(nameof(UserConversationDTO));
            this.AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            this.Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
        //     var stateAccessor = this.UserState.CreateProperty<T>(typeof(T).Name);
        //     var content = await stateAccessor.GetAsync(context, () => new T(), cancellationToken);
        //     return content;
        // }

        // public async Task SetUserState<T>(ITurnContext context, T content, CancellationToken cancellationToken)
        // {
        //     var stateAccessor = this.UserState.CreateProperty<T>(typeof(T).Name);
        //     await stateAccessor.SetAsync(context, content, cancellationToken);
        // }

        // public async Task<T> GetConversationState<T>(ITurnContext context, CancellationToken cancellationToken) where T : new()
        // {
        //     var stateAccessor = this.ConversationState.CreateProperty<T>(typeof(T).Name);
        //     var content = await stateAccessor.GetAsync(context, () => new T(), cancellationToken);
        //     return content;
        // }

        // public async Task SetConversationState<T>(ITurnContext context, T content, CancellationToken cancellationToken)
        // {
        //     var stateAccessor = this.ConversationState.CreateProperty<T>(typeof(T).Name);
        //     await stateAccessor.SetAsync(context, content, cancellationToken);
        // }

        // https://www.microsoftpressstore.com/articles/article.aspx?p=2854377&seqNum=3
        public async Task SendTypingActivity(ITurnContext context, CancellationToken cancellationToken)
        {
            // var typingActivity = Activity.CreateTypingActivity();

            // typingActivity.ReplyToId = context.Activity.Id;
            // typingActivity.From = new ChannelAccount
            // {
            //     Id = context.Activity.Recipient.Id,
            //     Name = context.Activity.Recipient.Name
            // };
            // typingActivity.Recipient = new ChannelAccount
            // {
            //     Id = context.Activity.From.Id,
            //     Name = context.Activity.From.Name
            // };
            // typingActivity.Conversation = new ConversationAccount
            // {
            //     Id = context.Activity.Conversation.Id,
            //     Name = context.Activity.Conversation.Name,
            //     IsGroup = context.Activity.Conversation.IsGroup
            // };

            // // send typing activity
            // await context.SendActivityAsync(typingActivity, cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

    }
}