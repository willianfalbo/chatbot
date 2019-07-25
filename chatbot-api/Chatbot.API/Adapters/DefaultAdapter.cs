using System;
using Chatbot.Common.Configuration;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Solutions.Middleware;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;

namespace Chatbot.Api.Adapters
{
    public class DefaultAdapter : BotFrameworkHttpAdapter
    {
        public DefaultAdapter(
            ICredentialProvider credentialProvider,
            ILogger<BotFrameworkHttpAdapter> logger,
            AppSettings appSettings,
            BotStateSet botStateSet,
            ConversationState conversationState = null)
            : base(credentialProvider)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError($"Exception caught : {exception.Message}");

                // Send a catch-all apology to the user.
                await turnContext.SendActivityAsync("Desculpe, parece que ocorreu um erro.");
            };

            Use(new ShowTypingMiddleware());
            Use(new SetLocaleMiddleware(appSettings.DefaultLocale ?? "pt-BR"));
            Use(new AutoSaveStateMiddleware(botStateSet));
        }
    }
}
