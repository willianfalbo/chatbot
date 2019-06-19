using System.IO;
using Chatbot.Common.Handlers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Chatbot.API.Extensions
{
    public class CustomComponentDialog : ComponentDialog
    {
        public CustomComponentDialog(string dialogId) : base(dialogId)
        {
        }

        protected Attachment CreateAdaptiveCardAttachment(string filePath, object data = null)
        {
            var adaptiveCardJson = string.Empty;

            if (data != null)
                adaptiveCardJson = TemplateHandler.RenderFile(filePath, data);
            else
                adaptiveCardJson = File.ReadAllText(filePath);

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            
            return adaptiveCardAttachment;
        }
    }
}