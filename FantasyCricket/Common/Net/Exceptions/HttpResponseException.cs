using System.Net.Http.Headers;

namespace System.Net.Http
{
    // Borrowed from NET.core
    public class HttpResponseException : Exception
    {
        private HttpResponseMessage httpResponseMessage;

        public HttpStatusCode HttpStatusCode { get; private set; }
        public string Content { get; private set; }
        public HttpResponseHeaders HttpResponseHeaders { get; private set; }

        public HttpResponseException(HttpStatusCode httpStatusCode, string reasonPhrase, string content, HttpResponseHeaders httpResponseHeaders)
            : base(reasonPhrase)
        {
            HttpStatusCode = httpStatusCode;
            Content = content;
            HttpResponseHeaders = httpResponseHeaders;
        }
    }
}
