using System.Threading.Tasks;
using Chatbot.Model.Manager;

namespace Chatbot.Common.Interfaces
{
    public interface IDirectLineManager
    {
        Task<ManagerResponse<DirectLineToken>> GenerateToken();
    }
}