using System;
using System.Threading.Tasks;
using Chatbot.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.BotBuilderSamples
{
    [Route("api/directline")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAppSettings _appSettings;
        private readonly IDirectLineManager _directLineManager;
        public AuthController(IAppSettings appSettings, IDirectLineManager directLineManager)
        {
            this._directLineManager = directLineManager ?? throw new System.ArgumentNullException(nameof(directLineManager));
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
        }

        // api/directline/token
        [HttpPost("token")]
        public async Task<IActionResult> Token()
        {
            var result = await _directLineManager.GenerateToken();

            if (result.HasError)
                return BadRequest(result.Errors);
            else
                return Ok(new
                {
                    conversationId = result.Value.ConversationId,
                    token = result.Value.Token,
                    expires_in = result.Value.ExpiresIn,
                    userID = $"dl_{Guid.NewGuid()}"
                });
        }
    }
}