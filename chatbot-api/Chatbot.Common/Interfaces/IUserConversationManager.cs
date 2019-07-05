using System.Threading.Tasks;
using Chatbot.Model.Manager;

namespace Chatbot.Common.Interfaces
{
    public interface IUserConversationManager
    {
        Task<ManagerResponse<UserConversation>> GetAsync(string userId);
        Task<ManagerResponse<UserConversation>> SaveAsync(UserConversation conversation);
    }
}