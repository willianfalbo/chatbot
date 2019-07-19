namespace Chatbot.Model.Service
{
    using Newtonsoft.Json;

    public partial class DirectLineTokenResponse
    {
        [JsonProperty("conversationId", NullValueHandling = NullValueHandling.Ignore)]
        public string ConversationId { get; set; }

        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }

        [JsonProperty("expires_in", NullValueHandling = NullValueHandling.Ignore)]
        public long? ExpiresIn { get; set; }
    }
}
