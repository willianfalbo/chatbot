namespace Chatbot.Common.Interfaces
{
    public interface IAppSettings
    {
        string ReceitaServiceApiUrl { get; }
        string ReceitaServiceToken { get; }
        string WebUiAppUrl { get; }
        string AzureDirectLineSecret { get; } // https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-directline?view=azure-bot-service-4.0
        string AzureDirectLineGenerateTokenUrl { get; } // https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-authentication?view=azure-bot-service-4.0
    }
}
