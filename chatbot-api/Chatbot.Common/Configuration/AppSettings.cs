using System;

namespace Chatbot.Common.Configuration
{
    public class AppSettings
    {
        public string DefaultLocale { get; set; }

        public string MicrosoftAppId { get; set; }
        public string MicrosoftAppPassword { get; set; }

        public string WebUiAppUrl { get; set; }

        // receita service
        public string ReceitaServiceUrl { get; set; }
        public string ReceitaServiceToken { get; set; }

        // azure direct line
        public string AzureDirectLineSecret { get; set; } // https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-directline?view=azure-bot-service-4.0
        public string AzureDirectLineGenerateTokenUrl { get; set; } // https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-authentication?view=azure-bot-service-4.0

        // document db - json server fake api
        public string DocumentDbEndpointURI { get; set; }

        // cosmos db settings
        public CosmosDbSettings CosmosDb { get; set; }

        // blob storage settings
        public BlobStorageSettings BlobStorage { get; set; }
    }

    public class CosmosDbSettings
    {
        public string PartitionKey { get; set; }
        public Uri CosmosDBEndpoint { get; set; }
        public string AuthKey { get; set; }
        public string DatabaseId { get; set; }
        public string CollectionId { get; set; }
    }

    public class BlobStorageSettings
    {
        public string ConnectionString { get; set; }
        public string Container { get; set; }
    }
}
