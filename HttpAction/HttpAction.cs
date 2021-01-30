using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace HttpAction
{
    public class HttpAction<T> : HttpRequestMessage
    {
        public string Host { get; set; }
        public string Path { get; set; }
        public HttpQueryCollection Queries { get; set; }
        public HttpHeaderCollection RequestHeaders { get; set; }

        public Func<HttpResponseMessage, Task<T>> ResponseHandler { get; set; }
        public Func<HttpResponseMessage, Task<T>> ErrorHandler { get; set; }

        public virtual Uri CreateUri()
        {
            var u = new UriBuilder();
            u.Host = this.Host;
            u.Path = this.Path;
            u.Query = this.Queries?.BuildQuery();
            return u.Uri;
        }
    }
}
