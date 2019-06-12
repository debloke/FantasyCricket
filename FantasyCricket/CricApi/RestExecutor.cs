using Common.Net.Extensions;
using FantasyCricket.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace FantasyCricket.KeyManager
{
    public class RestExecutor<T>
    {
        string[] apikeys = new string[] { "ZlrRCAEEwjg9Vknh9hOgVlV17ls2", "DofrC9fWV9faTciSbk8dJ6C4qYp2", "NcGcKABK2NhhC10hg2Hq7evazwk1", "Bj6DUlrcGvh8ltX61JTXvRsIMWH3", "AXeT9FGRcrXtncnYHhFX51zfKTk2", "zSprTGYBodM0Dwqt3xxDFYHFChl1", "WxAwFrBwg1d2RZYvVgkGk9TKE8K2" };
        private readonly HttpClient httpClient = new HttpClient();

        public T Invoke(string url)
        {

            int attempt = 0;
            string response = null;

            while (attempt < apikeys.Length)
            {
                response = httpClient.InvokeGet($"{url}apikey={apikeys[attempt]}");
                CricApiError error = JsonConvert.DeserializeObject<CricApiError>(response, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                if (String.IsNullOrEmpty(error.error))
                {
                    return JsonConvert.DeserializeObject<T>(response, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
                else
                {
                    attempt++;
                    //Do Nothing and retry with another key if there are more keys
                    if (attempt == apikeys.Length)
                    {
                        throw new Exception(response);
                    }
                }
            }

            return default(T);
        }
    }

}

