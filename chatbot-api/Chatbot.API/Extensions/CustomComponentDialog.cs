using Microsoft.Bot.Builder.Dialogs;

namespace Chatbot.Api.Extensions
{
    public class CustomComponentDialog : ComponentDialog
    {
        public CustomComponentDialog(string dialogId)
            : base(dialogId)
        {
        }
    }
}
