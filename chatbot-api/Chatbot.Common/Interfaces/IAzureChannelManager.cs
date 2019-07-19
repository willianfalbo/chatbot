using System.Threading.Tasks;
using Chatbot.Model.Manager;

namespace Chatbot.Common.Interfaces
{
    public interface IAzureChannelManager
    {
        Task<ManagerResponse<DirectLineToken>> DirectLineToken();
    }
}