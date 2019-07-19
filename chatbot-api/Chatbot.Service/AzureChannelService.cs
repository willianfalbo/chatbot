using System.Threading.Tasks;
using Chatbot.Common.Interfaces;
using Chatbot.Model.Service;
using Chatbot.Service.Util;

namespace Chatbot.Service
{
    public class AzureChannelService : ServiceBase
    {
        protected override bool _supportGzipCompression => false;
        protected override bool _supportBearerToken => true;
        private readonly IAppSettings _appSettings;

        public AzureChannelService(IAppSettings appSettings) : base()
        {
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
        }

        public async Task<ApiResponse<DirectLineTokenResponse, dynamic>> DirectLineToken()
        {
            base._accessToken = this._appSettings.AzureDirectLineSecret;
            
            return await base.Post<DirectLineTokenResponse, dynamic>(
                this._appSettings.AzureDirectLineGenerateTokenUrl, null
            );
        }

    }
}
