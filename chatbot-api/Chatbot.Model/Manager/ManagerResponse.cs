using System.Collections.Generic;
using System.Linq;

namespace Chatbot.Model.Manager
{
    public class ManagerResponse<T>
    {
        public ManagerResponse(T value) => Value = value;

        public ManagerResponse(string errorMessage)
        {
            HasError = true;
            Errors = new List<ErrorItem> { new ErrorItem(errorMessage) };
        }

        public ManagerResponse(List<string> errorMessages)
        {
            HasError = true;
            Errors = errorMessages.Select(error => new ErrorItem(error)).ToList();
        }

        public ManagerResponse(ErrorItem errorItem)
        {
            HasError = true;
            Errors = new List<ErrorItem> { errorItem };
        }

        public ManagerResponse(List<ErrorItem> errorItems)
        {
            HasError = true;
            Errors = errorItems;
        }

        public T Value { get; }
        public bool HasError { get; } = false;
        public List<ErrorItem> Errors { get; } = null;
    }

    public class ErrorItem
    {
        public ErrorItem(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new System.ArgumentNullException(nameof(message));

            Message = message;
        }

        public string Message { get; }
    }
}