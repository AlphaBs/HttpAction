using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace HttpAction
{
    public class HttpActionRequest<T> : HttpRequestMessage
    {
        public Func<string, T> ResponseHandler { get; set; }
    }
}
