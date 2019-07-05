using AutoMapper;
using Chatbot.API.DTO;
using Chatbot.Model.Manager;

namespace Chatbot.API.Mappings
{
    public class DTOMappingProfile : Profile
    {
        public DTOMappingProfile()
        {
            //user conversation
            CreateMap<UserPreferenceDTO, UserPreference>().ReverseMap();
            CreateMap<UserProfileDTO, UserProfile>().ReverseMap();
            CreateMap<UserCompanyDTO, UserCompany>().ReverseMap();
            CreateMap<FamilyIncomeDTO, FamilyIncome>().ReverseMap();
            CreateMap<FamilyExpenseDTO, FamilyExpense>().ReverseMap();
            CreateMap<UserSocioEconomicDTO, UserSocioEconomic>().ReverseMap();
            CreateMap<UserConversationDTO, UserConversation>().ReverseMap();
        }
    }
}