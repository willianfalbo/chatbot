using Microsoft.Bot.Builder.Dialogs;

namespace Chatbot.API.Extensions
{
    public class CustomComponentDialog : ComponentDialog
    {
        public CustomComponentDialog(string dialogId)
            : base(dialogId)
        {
        }
    }
}
