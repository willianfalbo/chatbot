using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Chatbot.Common.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Chatbot.API.Helpers
{
    public interface IDialogHelper
    {
        UserState _userState { get; }
        ConversationState _conversationState { get; }
        IAppSettings _appSettings { get; }
        IMapper _mapper { get; }
        IUserConversationManager _userConversationManager { get; }

        Attachment CreateAdaptiveCardAttachment(string filePath, object data = null);
        // Task<T> GetUserState<T>(ITurnContext context, CancellationToken cancellationToken) where T : new();
        // Task SetUserState<T>(ITurnContext context, T content, CancellationToken cancellationToken);
        Task<T> GetConversationState<T>(ITurnContext context, CancellationToken cancellationToken) where T : new();
        Task SetConversationState<T>(ITurnContext context, T content, CancellationToken cancellationToken);
        Task SendTypingActivity(ITurnContext context, CancellationToken cancellationToken);
        Task SaveConversationDB(ITurnContext context, CancellationToken cancellationToken);
    }
}