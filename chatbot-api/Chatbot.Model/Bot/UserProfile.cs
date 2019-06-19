namespace Chatbot.Model.Bot
{
    using System.Collections.Generic;

    public class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public bool AcceptedAgreement { get; set; }
        public bool WantsToProvideAge { get; set; }
    }
}
