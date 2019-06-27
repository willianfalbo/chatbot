using AutoMapper;
using Chatbot.Model.Service;
using Chatbot.Model.Manager;

namespace Chatbot.Manager.Mappings
{
    public class DirectLineTokenMappingProfile : Profile
    {
        public DirectLineTokenMappingProfile()
        {
            CreateMap<DirectLineTokenResponse, DirectLineToken>();
        }
    }
}