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

        public string ReceitaServiceApiUrl => this._configuration.GetValue<string>("ReceitaServiceUrl", null);
        public string ReceitaServiceToken => this._configuration.GetValue<string>("ReceitaServiceToken", null);
        public string DefaultRootUrl => this._configuration.GetValue<string>("DefaultRootUrl", null);
    }
}
