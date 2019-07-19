using System;
using System.Threading.Tasks;
using Chatbot.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.BotBuilderSamples
{
    // api/azurechannel
    [Route("api/azurechannel")]
    [ApiController]
    public class AzureChannelController : ControllerBase
    {
        private readonly IAppSettings _appSettings;
        private readonly IAzureChannelManager _azureChannelManager;
        public AzureChannelController(IAppSettings appSettings, IAzureChannelManager azureChannelManager)
        {
            this._azureChannelManager = azureChannelManager ?? throw new System.ArgumentNullException(nameof(azureChannelManager));
            this._appSettings = appSettings ?? throw new System.ArgumentNullException(nameof(appSettings));
        }

        // api/azurechannel/directline/token
        [HttpPost("directline/token")]
        public async Task<IActionResult> DirectLineToken()
        {
            var result = await _azureChannelManager.DirectLineToken();

            if (result.HasError)
                return BadRequest(result);
            else
                return Ok(result);
        }
    }
}