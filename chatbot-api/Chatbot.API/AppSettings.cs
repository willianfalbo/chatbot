using System;
using Chatbot.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Chatbot.API
{
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _configuration;
        public AppSettings(IConfiguration configuration)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string WebUiAppUrl => this._configuration.GetValue<string>("WebUiAppUrl", null);

        // receita service
        public string ReceitaServiceApiUrl => this._configuration.GetValue<string>("ReceitaServiceUrl", null);
        public string ReceitaServiceToken => this._configuration.GetValue<string>("ReceitaServiceToken", null);

        // azure direct line
        public string AzureDirectLineSecret => this._configuration.GetValue<string>("AzureDirectLineSecret", null);
        public string AzureDirectLineGenerateTokenUrl => this._configuration.GetValue<string>("AzureDirectLineGenerateTokenUrl", null);

        //document db - json server fake api
        public string DocumentDbEndpointURI => this._configuration.GetValue<string>("DocumentDbEndpointURI", null);
    }
}
