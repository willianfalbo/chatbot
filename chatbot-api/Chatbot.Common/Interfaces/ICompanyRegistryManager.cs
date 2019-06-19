using System.Threading.Tasks;
using Chatbot.Model.Manager;

namespace Chatbot.Common.Interfaces
{
    public interface ICompanyRegistryManager
    {
        Task<ManagerResponse<CompanyRegistry>> SearchForCnpj(string cnpj);
    }
}