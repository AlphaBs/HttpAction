using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HttpAction;

namespace HttpActionTest
{
    class User : ActionResponse
    {
        public string Name { get; set; }
        public string ID { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            User response = await client.SendActionAsync(new HttpAction<User>
            {
                Method = HttpMethod.Post,
                Host = "https://webhook.site",
                Path = "a753bce4-f1a0-4299-b816-53f1581af8cd",
                RequestHeaders = new HttpHeaderCollection
                {
                    { "h1", "askdjfklasdf" },
                    { "Accept-Language", "ko-KR" }
                },
                Queries = new HttpQueryCollection
                {
                    { "what", "hi hi hell" },
                    { "what", "fffffff" },
                    { "22", "lks noon jkl(((((**" }
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "hell", " #$R(UWA)(URC(WA#C CA(M CR#W$(CAW#R " },
                    { "ff", "234123134" }
                }),
                ResponseHandler = async (response) =>
                {
                    var res = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(res);
                    return new User();
                }
            });
            Console.WriteLine(response.StatusCode);
        }
    }
}
