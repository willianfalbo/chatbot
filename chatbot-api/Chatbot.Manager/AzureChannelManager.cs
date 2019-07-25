using System;
using System.Threading.Tasks;
using AutoMapper;
using Chatbot.Common.Configuration;
using Chatbot.Common.Interfaces;
using Chatbot.Model.Manager;
using Chatbot.Service;

namespace Chatbot.Manager
{
    public class AzureChannelManager : IAzureChannelManager
    {
        private readonly AppSettings _appSettings;
        private readonly AzureChannelService _azureChannelService;
        private readonly IMapper _mapper;

        public AzureChannelManager(AppSettings appSettings, IMapper mapper)
        {
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._azureChannelService = new AzureChannelService(_appSettings);
        }

        public async Task<ManagerResponse<DirectLineToken>> DirectLineToken()
        {
            var response = await _azureChannelService.DirectLineToken();

            if (response.HasError) //TODO: Check if this is correct
                return new ManagerResponse<DirectLineToken>(response?.ErrorMessage); //TODO: wrapped up this into the Auto Mapper
            else
                return new ManagerResponse<DirectLineToken>(_mapper.Map<DirectLineToken>(response?.Content));
        }
    }
}