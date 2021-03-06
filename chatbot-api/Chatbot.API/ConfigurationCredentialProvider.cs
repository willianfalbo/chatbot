﻿using Chatbot.Common.Configuration;
using Microsoft.Bot.Connector.Authentication;

namespace Chatbot.Api
{
    public class ConfigurationCredentialProvider : SimpleCredentialProvider
    {
        public ConfigurationCredentialProvider(AppSettings appSettings)
            : base(appSettings.MicrosoftAppId, appSettings.MicrosoftAppPassword)
        {
        }
    }
}