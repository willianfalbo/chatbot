using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Chatbot.Common.Interfaces;
using Chatbot.Model.Manager;
using Chatbot.Model.Service;
using Chatbot.Service;

namespace Chatbot.Manager
{
    public class UserConversationManager : IUserConversationManager
    {
        private readonly IAppSettings _appSettings;
        private readonly UserConversationService _userConversationService;
        private readonly IMapper _mapper;

        public UserConversationManager(IAppSettings appSettings, IMapper mapper)
        {
            this._appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._userConversationService = new UserConversationService(_appSettings);
        }

        private async Task<ManagerResponse<UserConversation>> CreateAsync(UserConversation conversation)
        {
            var request = _mapper.Map<UserConversationReqResp>(conversation);

            var response = await this._userConversationService.SaveConversationAsync(request);

            if (response.HasError) //TODO: Check if this is correct
                return new ManagerResponse<UserConversation>(response?.ErrorMessage); //TODO: wrapped up this into the Auto Mapper
            else
                return new ManagerResponse<UserConversation>(_mapper.Map<UserConversation>(response?.Content));
        }

        private async Task<ManagerResponse<UserConversation>> UpdateAsync(UserConversation conversation)
        {
            var request = _mapper.Map<UserConversationReqResp>(conversation);

            var response = await this._userConversationService.UpdateConversationAsync(request);

            if (response.HasError) //TODO: Check if this is correct
                return new ManagerResponse<UserConversation>(response?.ErrorMessage); //TODO: wrapped up this into the Auto Mapper
            else
                return new ManagerResponse<UserConversation>(_mapper.Map<UserConversation>(response?.Content));
        }

        public async Task<ManagerResponse<UserConversation>> GetAsync(string userId)
        {
            var response = await this._userConversationService.GetConversationAsync(userId);

            // we expect only NotFound error. Otherwise, throw an exeption
            if (response.HasError)
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;
                else
                    throw new Exception($"Unexpected error when searching for UserID. Error Message: {response?.ErrorMessage}");
            else
                return new ManagerResponse<UserConversation>(_mapper.Map<UserConversation>(response?.Content));
        }

        public async Task<ManagerResponse<UserConversation>> SaveAsync(UserConversation conversation)
        {
            var current = await this.GetAsync(conversation?.UserId);

            // create or update
            var response = (ManagerResponse<UserConversation>)null;
            if (current == null)
                response = await this.CreateAsync(conversation);
            else
                response = await this.UpdateAsync(conversation);

            // we expect not to receive error up to this point
            if (response.HasError)
                throw new Exception($"Unexpected error when saving User Conversation. Error Message: {string.Join(", ", response?.Errors)}");
            else
                return response;
        }
    }
}
