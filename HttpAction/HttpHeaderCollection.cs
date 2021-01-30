using System;
using System.Collections.Generic;
using System.Text;

namespace MojangAPI.HttpAction
{
    public class HttpHeaderCollection
    {
        private Dictionary<string, List<string>> Headers = new Dictionary<string, List<string>>();

        public void Add(string key, string value)
        {
            if (Headers.ContainsKey(key))
                Headers[key].Add(value);
            else
            {
                List<string> values = new List<string>(1);
                values.Add(value);
                Headers[key] = values;
            }
        }

        public string BuildQuery()
        {
            return "?" + string.Join("&", this);
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var item in Headers)
            {
                if (string.IsNullOrEmpty(item.Key))
                {
                    foreach (var value in item.Value)
                    {
                        yield return HttpUtility.UrlEncode(value);
                    }
                }
                else
                {
                    foreach (var value in item.Value)
                    {
                        yield return $"{HttpUtility.UrlEncode(item.Key)}={HttpUtility.UrlEncode(value)}";
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
