using System.Threading.Tasks;
using Chatbot.Common.Interfaces;
using Chatbot.Model.Service;
using Chatbot.Service.Util;

namespace Chatbot.Service
{
    public class ReceitaService : ServiceBase
    {
        protected override bool _supportGzipCompression => false;
        protected override bool _supportBearerToken => false;
        private readonly IAppSettings _appSettings;

        public ReceitaService(IAppSettings appSettings) : base()
        {
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
            base._accessToken = this._appSettings.ReceitaServiceToken;
        }

        public async Task<ApiResponse<ReceitaServiceResponse, ReceitaServiceResponse>> SearchForCnpj(string cnpj) =>
            await base.Get<ReceitaServiceResponse, ReceitaServiceResponse>(
                $"{this._appSettings.ReceitaServiceApiUrl}/v1/cnpj/{cnpj}"
            );

    }
}
