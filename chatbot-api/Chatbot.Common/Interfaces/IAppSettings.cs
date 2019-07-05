namespace Chatbot.Common.Interfaces
{
    public interface IAppSettings
    {
        string WebUiAppUrl { get; }

        // receita service
        string ReceitaServiceApiUrl { get; }
        string ReceitaServiceToken { get; }

        // azure direct line
        string AzureDirectLineSecret { get; } // https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-directline?view=azure-bot-service-4.0
        string AzureDirectLineGenerateTokenUrl { get; } // https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-authentication?view=azure-bot-service-4.0

        // document db - json server fake api
        string DocumentDbEndpointURI { get; }
    }
}
