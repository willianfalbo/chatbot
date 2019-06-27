using System.Threading.Tasks;
using Chatbot.Common.Interfaces;
using Chatbot.Model.Service;
using Chatbot.Service.Util;

namespace Chatbot.Service
{
    public class DirectLineService : ServiceBase
    {
        protected override bool _supportGzipCompression => false;
        protected override bool _supportBearerToken => true;
        private readonly IAppSettings _appSettings;

        public DirectLineService(IAppSettings appSettings) : base()
        {
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
            base._accessToken = this._appSettings.AzureDirectLineSecret;
        }

        public async Task<ApiResponse<DirectLineTokenResponse, dynamic>> GenerateToken() =>
            await base.Post<DirectLineTokenResponse, dynamic>(
                this._appSettings.AzureDirectLineGenerateTokenUrl, null
            );

    }
}
