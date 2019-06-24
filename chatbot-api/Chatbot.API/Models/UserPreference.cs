using System.Collections.Generic;

namespace Chatbot.API.Models
{
    // contains information about the user state (used when they first start chatting)
    public class UserPreference
    {
        public string UserOption { get; set; }

        public static List<string> ChatbotOptions() =>
            new List<string> {
                UserHelpOption.MICROCREDITO,
                UserHelpOption.CAPITAL_GIRO,
                UserHelpOption.CAPITAL_GIRO_CARTORIO,
            };
    }
}