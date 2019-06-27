namespace Chatbot.Model.Manager
{
    public class DirectLineToken
    {
        public string ConversationId { get; set; }
        public string Token { get; set; }
        public long? ExpiresIn { get; set; }
    }
}