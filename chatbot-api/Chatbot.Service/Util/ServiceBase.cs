using Chatbot.Model.Service;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot.Service.Util
{
    public abstract class ServiceBase
    {
        protected abstract bool _supportGzipCompression { get; }
        protected abstract bool _supportBearerToken { get; }
        protected string _accessToken { get; set; }
        private readonly TimeSpan[] _pauseBetweenFailures;

        protected ServiceBase()
        {
            _pauseBetweenFailures = new[] {
                TimeSpan.FromSeconds(3), //1
                TimeSpan.FromSeconds(6), //2
                TimeSpan.FromSeconds(12), //3
                TimeSpan.FromSeconds(24), //4
            };
        }

        public async Task<ApiResponse> Get(string uri, Hashtable customHeaders = null)
        {
            using (var httpClient = CreateHttpClient())
            {
                if (customHeaders != null)
                    AddCustomHeaders(httpClient, customHeaders);

                var response = await httpClient.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return new ApiResponse(
                        content,
                        response.StatusCode
                    );
                else
                    return new ApiResponse(
                        content,
                        response.StatusCode,
                        true
                    );
            }
        }

        public async Task<ApiResponse<T, TError>> Get<T, TError>(string uri, Hashtable customHeaders = null)
        {
            using (var httpClient = CreateHttpClient())
            {
                if (customHeaders != null)
                    AddCustomHeaders(httpClient, customHeaders);

                var retryPolicy = Policy
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(_pauseBetweenFailures);

                var response = new HttpResponseMessage();
                await retryPolicy.ExecuteAsync(async () =>
                {
                    response = await httpClient.GetAsync(uri);
                });

                var content = await response.Content.ReadAsStringAsync();

                return DeserializeAndCheck<T, TError>(
                    response,
                    content
                );
            }
        }

        public async Task<ApiResponse<T, TError>> Post<T, TError>(string uri, object input, Hashtable customHeaders = null)
        {
            using (var httpClient = CreateHttpClient())
            {
                if (customHeaders != null)
                    AddCustomHeaders(httpClient, customHeaders);

                var request =
                    new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json");

                var retryPolicy = Policy
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(_pauseBetweenFailures);

                var response = new HttpResponseMessage();
                await retryPolicy.ExecuteAsync(async () =>
                {
                    response = await httpClient.PostAsync(uri, request);
                });

                var content = await response.Content.ReadAsStringAsync();

                return DeserializeAndCheck<T, TError>(
                    response,
                    content
                );
            }
        }

        public async Task<T> Put<T>(string uri, T input)
        {
            using (var httpClient = CreateHttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Put, uri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json")
                };
                request.Properties["RequestTimeout"] = TimeSpan.FromSeconds(300);

                try
                {
                    var response = await httpClient.SendAsync(request);
                    var results = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(results);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public async Task<bool> Delete(string uri)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.DeleteAsync(uri);
                return response.StatusCode == System.Net.HttpStatusCode.OK
                    || response.StatusCode == System.Net.HttpStatusCode.NoContent;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = _supportGzipCompression
                ? new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
                })
                : new HttpClient();
            AddHeaders(client);
            return client;
        }

        private void AddHeaders(HttpClient httpClient)
        {
            if (_supportGzipCompression)
            {
                new List<string> { "gzip", "deflate" }.ForEach(encoding =>
                 {
                     httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(encoding));
                 });
            }

            if (_supportBearerToken && !string.IsNullOrWhiteSpace(_accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }
        }

        private void AddCustomHeaders(HttpClient httpClient, Hashtable headers)
        {
            foreach (DictionaryEntry header in headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key.ToString(), header.Value.ToString());
            }
        }

        private static ApiResponse<T, TError> DeserializeAndCheck<T, TError>(HttpResponseMessage response, string content)
        {
            var responseType = response?.Content?.Headers?.ContentType?.MediaType;
            var isSuccess = response.IsSuccessStatusCode;

            // deserialize json
            if (responseType == "application/json")
            {
                if (isSuccess)
                {
                    return new ApiResponse<T, TError>(
                        JsonConvert.DeserializeObject<T>(content),
                        response.StatusCode
                    );
                }
                else
                {
                    return new ApiResponse<T, TError>(
                        JsonConvert.DeserializeObject<TError>(content),
                        response.StatusCode
                    );
                }
            }
            // deserialize xml (TODO: Not implemented yet as we don't use it so far)
            else if ((new List<string> { "application/xml", "text/xml" }).Contains(responseType))
            {
                throw new NotImplementedException($"Not implemented {responseType} deserialization.");
            }
            // otherwise, we consider error
            else
            {
                return new ApiResponse<T, TError>(
                    content,
                    response.StatusCode
                );
            }
        }

    }
}
