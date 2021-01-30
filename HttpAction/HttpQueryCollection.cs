using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace MojangAPI.HttpAction
{
    public class HttpQueryCollection : IEnumerable<string>
    {
        private Dictionary<string, string> Queries = new Dictionary<string, string>();

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var item in Queries)
            {
                if (string.IsNullOrEmpty(item.Key))
                    yield return HttpUtility.UrlEncode(item.Value);
                else
                    yield return $"{HttpUtility.UrlEncode(item.Key)}={HttpUtility.UrlEncode(item.Value)}";
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
