using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Chatbot.Common.Configuration;
using Chatbot.Common.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Chatbot.API.Helpers
{
    public interface IDialogHelper
    {
        UserState UserState { get; }
        ConversationState ConversationState { get; }
        IStatePropertyAccessor<UserConversationDTO> UserAccessor { get; }
        AppSettings AppSettings { get; }
        IMapper Mapper { get; }

        Attachment CreateAdaptiveCardAttachment(string filePath, object data = null);
        // Task<T> GetUserState<T>(ITurnContext context, CancellationToken cancellationToken) where T : new();
        // Task SetUserState<T>(ITurnContext context, T content, CancellationToken cancellationToken);
        // Task<T> GetConversationState<T>(ITurnContext context, CancellationToken cancellationToken) where T : new();
        // Task SetConversationState<T>(ITurnContext context, T content, CancellationToken cancellationToken);
        Task SendTypingActivity(ITurnContext context, CancellationToken cancellationToken);
    }
}