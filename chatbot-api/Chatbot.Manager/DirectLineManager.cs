using System;
using System.Threading.Tasks;
using AutoMapper;
using Chatbot.Common.Interfaces;
using Chatbot.Model.Manager;
using Chatbot.Service;

namespace Chatbot.Manager
{
    public class DirectLineManager : IDirectLineManager
    {
        private readonly IAppSettings _appSettings;
        private readonly DirectLineService _directLineService;
        private readonly IMapper _mapper;

        public DirectLineManager(IAppSettings appSettings, IMapper mapper)
        {
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._directLineService = new DirectLineService(_appSettings);
        }

        public async Task<ManagerResponse<DirectLineToken>> GenerateToken()
        {
            var response = await _directLineService.GenerateToken();

            // if (response.HasError)
            //     return new ManagerResponse<DirectLineToken>(response?.Content?.Message); //TODO: wrapped up this into the Auto Mapper
            // else
            return new ManagerResponse<DirectLineToken>(_mapper.Map<DirectLineToken>(response?.Content));
        }
    }
}