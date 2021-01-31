using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttpAction
{
    public class HttpResponseHandlers
    {
        public static Func<HttpResponseMessage, Task<T>> GetDefaultResponseHandler<T>()
        {
            if (typeof(T) == typeof(bool))
                return GetSuccessCodeResponseHandler() as Func<HttpResponseMessage, Task<T>>;
            if (typeof(T) == typeof(int))
                return GetStatusCodeResponseHandler()  as Func<HttpResponseMessage, Task<T>>;
            if (typeof(T) == typeof(string))
                return GetStringResponseHandler()      as Func<HttpResponseMessage, Task<T>>;
            if (typeof(T) == typeof(Stream))
                return GetStreamResponseHandler()      as Func<HttpResponseMessage, Task<T>>;

            return GetJsonHandler<T>();
        }

        public static Func<HttpResponseMessage, Task<bool>> GetSuccessCodeResponseHandler() =>
            (response) =>
            {
                bool result = (int)response.StatusCode / 100 == 2;
                return Task.FromResult(result);
            };

        public static Func<HttpResponseMessage, Task<bool>> GetSuccessCodeResponseHandler(int successCode) =>
            (response) =>
            {
                bool result = (int)response.StatusCode == successCode;
                return Task.FromResult(result);
            };

        public static Func<HttpResponseMessage, Task<int>> GetStatusCodeResponseHandler() =>
            (response) =>
            {
                return Task.FromResult((int)response.StatusCode);
            };

        public static Func<HttpResponseMessage, Task<string>> GetStringResponseHandler() =>
            (response) =>
            {
                return response.Content.ReadAsStringAsync();
            };

        public static Func<HttpResponseMessage, Task<Stream>> GetStreamResponseHandler() =>
            (response) =>
            {
                return response.Content.ReadAsStreamAsync();
            };

        public static Func<HttpResponseMessage, Task<T>> GetJsonHandler<T>() =>
            async (response) =>
            {
                string res = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(res, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            };

        public static Func<HttpResponseMessage, Task<T>> GetDefaultErrorHandler<T>() =>
            (response) =>
            {
                response.EnsureSuccessStatusCode();
                return Task.FromResult(default(T));
            };

        private static JsonSerializer defaultSerializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static Func<HttpResponseMessage, Task<T[]>> GetJsonArrayHandler<T>() =>
            async (response) =>
            {
                string res = await response.Content.ReadAsStringAsync();
                JArray jarr = JArray.Parse(res);
                return jarr.Select(x => x.ToObject<T>(defaultSerializer)).ToArray();
            };
    }
}
