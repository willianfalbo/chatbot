using System;
using System.Threading.Tasks;
using AutoMapper;
using Chatbot.Common.Interfaces;
using Chatbot.Common.Extensions;
using Chatbot.Model.Manager;
using Chatbot.Service;
using Chatbot.Common.Configuration;

namespace Chatbot.Manager
{
    public class CompanyRegistryManager : ICompanyRegistryManager
    {
        private readonly AppSettings _appSettings;
        private readonly ReceitaService _receitaService;
        private readonly IMapper _mapper;

        public CompanyRegistryManager(AppSettings appSettings, IMapper mapper)
        {
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._receitaService = new ReceitaService(_appSettings);
        }

        public async Task<ManagerResponse<CompanyRegistry>> SearchForCnpj(string cnpj)
        {
            cnpj = cnpj.Digits();

            if (!cnpj.IsCnpjValid())
                return new ManagerResponse<CompanyRegistry>("CNPJ Inválido!");

            var response = await _receitaService.SearchForCnpj(cnpj);

            if (!response.HasError && response?.Content?.Status?.ToUpper() == "ERROR")
                return new ManagerResponse<CompanyRegistry>(response?.Content?.Message); //TODO: wrapped up this into the Auto Mapper
            else if (response.HasError)
                return new ManagerResponse<CompanyRegistry>(response?.ErrorMessage);
            else
                return new ManagerResponse<CompanyRegistry>(_mapper.Map<CompanyRegistry>(response?.Content));
        }
    }
}