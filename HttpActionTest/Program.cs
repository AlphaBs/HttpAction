using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HttpAction;

namespace HttpActionTest
{
    class User
    {
        public string Name { get; set; }
        public string ID { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

        }
    }
}
