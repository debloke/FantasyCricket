using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Net.Extensions
{
    public static class HttpClientExtension
    {
        public static string InvokeGet(this HttpClient client, string requestUri)
        {
            return InvokeGet(client, requestUri, CancellationToken.None);
        }


        public static string InvokeGeneric(this HttpClient client, HttpMethod method, Uri uri, HttpContent content)
        {
            using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, uri))
            {
                httpRequestMessage.Content = content;
                using (var response = client.SendAsync(httpRequestMessage).Result)
                {
                    return ReadResponseAsString(response);
                }
            }
        }

        public static string InvokeGet(this HttpClient client, string requestUri, CancellationToken cancellationToken)
        {
            return InvokeWebOperation<string>(() =>
            {
                using (var response = client.GetAsync(requestUri, cancellationToken).Result)
                {
                    return ReadResponseAsString(response);
                }
            });
        }

        public static string InvokePut(this HttpClient client, string requestUri, string content, string contentType = "application/json")
        {
            return InvokePut(client, requestUri, content, contentType, CancellationToken.None);
        }

        public static string InvokePut(this HttpClient client, string requestUri, string content, string contentType, CancellationToken cancellationToken)
        {
            return InvokeWebOperation<string>(() =>
            {
                using (var stringContent = new StringContent(content))
                {
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                    using (var response = client.PutAsync(requestUri, stringContent, cancellationToken).Result)
                    {
                        return ReadResponseAsString(response);
                    }
                }
            });
        }

        public static string InvokePost(this HttpClient client, string requestUri, string content, string contentType = "application/json")
        {
            return InvokePost(client, requestUri, content, contentType, CancellationToken.None);
        }

        public static string InvokePost(this HttpClient client, string requestUri, string content, string contentType, CancellationToken cancellationToken)
        {
            return InvokeWebOperation<string>(() =>
            {
                using (var stringContent = new StringContent(content))
                {
                    stringContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                    using (var response = client.PostAsync(requestUri, stringContent, cancellationToken).Result)
                    {
                        return ReadResponseAsString(response);
                    }
                }
            });
        }

        public static string InvokeDelete(this HttpClient client, string requestUri)
        {
            return InvokeDelete(client, requestUri, CancellationToken.None);
        }

        public static string InvokeDelete(this HttpClient client, string requestUri, CancellationToken cancellationToken)
        {
            return InvokeWebOperation<string>(() =>
            {
                using (HttpResponseMessage response = client.DeleteAsync(requestUri, cancellationToken).Result)
                {
                    return ReadResponseAsString(response);
                }
            });
        }

        private static string ReadResponseAsString(HttpResponseMessage response)
        {
            using (HttpContent content = response.Content)
            {
                string responseContent = content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpResponseException(response.StatusCode, response.ReasonPhrase, responseContent, response.Headers);
                }

                return responseContent;
            }
        }

        private static TResult InvokeWebOperation<TResult>(Func<TResult> WebOperation)
        {
            bool isRetryAttempted = false;

            while (true)
            {
                try
                {
                    return WebOperation();
                }
                catch (AggregateException exception)
                {
                    if (isRetryAttempted)
                    {
                        exception.Handle((e) =>
                        {
                            if (e is TaskCanceledException)
                            {
                                throw new HttpRequestException("The connection has timed out", e);
                            }

                            throw new HttpRequestException(
                                ((e.InnerException != null) ? e.Message + ". " + e.InnerException.Message : e.Message), e);
                        });

                        throw new HttpRequestException(exception.Message, exception);
                    }

                    // Throw exception on next failed attempt
                    isRetryAttempted = true;
                }
            }
        }
    }
}