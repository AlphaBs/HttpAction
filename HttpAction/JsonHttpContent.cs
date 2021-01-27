using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpAction
{
    public class JsonHttpContent : HttpContent
    {
        public JsonHttpContent(object obj)
        {

        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            context.
        }

        protected override bool TryComputeLength(out long length)
        {
            throw new NotImplementedException();
        }
    }
}
