using System.Threading.Tasks;
using Chatbot.Common.Configuration;
using Chatbot.Model.Service;
using Chatbot.Service.Util;

namespace Chatbot.Service
{
    public class UserConversationService : ServiceBase
    {
        protected override bool _supportGzipCompression => false;
        protected override bool _supportBearerToken => false;
        private readonly AppSettings _appSettings;

        public UserConversationService(AppSettings appSettings) : base()
        {
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
            // base._accessToken = appSettings.TOKEN;
        }

        public async Task<ApiResponse<UserConversationReqResp, dynamic>> SaveConversationAsync(UserConversationReqResp request) =>
            await base.Post<UserConversationReqResp, dynamic>($"{_appSettings.DocumentDbEndpointURI}/chatbot", request);

        public async Task<ApiResponse<UserConversationReqResp, dynamic>> UpdateConversationAsync(UserConversationReqResp request) =>
            await base.Put<UserConversationReqResp, dynamic>($"{_appSettings.DocumentDbEndpointURI}/chatbot/{request.UserId}", request);

        public async Task<ApiResponse<UserConversationReqResp, dynamic>> GetConversationAsync(string userId) =>
            await base.Get<UserConversationReqResp, dynamic>($"{_appSettings.DocumentDbEndpointURI}/chatbot/{userId}");
    }
}
