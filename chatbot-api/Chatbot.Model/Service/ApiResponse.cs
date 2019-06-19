using System.Net;
using System.Net.Http.Headers;

namespace Chatbot.Model.Service
{
    public class ApiResponse
    {
        public ApiResponse(string content, HttpStatusCode statusCode, bool hasError = false)
        {
            if (content == null)
                throw new System.ArgumentNullException(nameof(content));

            this.Content = content;
            this.StatusCode = statusCode;
            this.HasError = hasError;
        }

        public string Content { get; }
        public bool HasError { get; } = false;
        public HttpStatusCode StatusCode { get; }
    }

    public class ApiResponse<T, TError>
    {
        public ApiResponse(T content, HttpStatusCode statusCode)
        {
            if (content == null)
                throw new System.ArgumentNullException(nameof(content));

            this.Content = content;
            this.StatusCode = statusCode;
        }

        public ApiResponse(TError contentError, HttpStatusCode statusCode)
        {
            if (contentError == null)
                throw new System.ArgumentNullException(nameof(contentError));

            this.ContentError = contentError;
            this.HasError = true;
            this.StatusCode = statusCode;
        }

        public ApiResponse(string errorMessage, HttpStatusCode statusCode)
        {
            if (errorMessage == null)
                throw new System.ArgumentNullException(nameof(errorMessage));

            this.ErrorMessage = errorMessage;
            this.HasError = true;
            this.StatusCode = statusCode;
        }

        public T Content { get; }
        public TError ContentError { get; }
        public string ErrorMessage { get; }
        public bool HasError { get; } = false;
        public HttpStatusCode StatusCode { get; }
    }
}