# HttpAction
Http client for .NET

----------

[HttpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0) 의 확장 클래스와 여러 유틸 클래스가 있습니다.

### 만든이유

하나의 식으로 async HTTP API 요청과 응답 처리를 하고싶어서  

### 사용법

먼저 응답 데이터를 표현할 모델 클래스를 만들어 줍니다.

```csharp
class UserInfo
{
  public string Name { get; set; }
  public string ID { get; set; }
  public int Age { get; set; }
}
```

아래 코드는 HTTP 요청 후 응답을 JSON 형태로 파싱하고 UserInfo 클래스로 역직렬화해줍니다.

```csharp
HttpClient client = new HttpClient();
UserInfo response = await client.SendActionAsync(new HttpAction<UserInfo>
{
  Method = HttpMethod.Get,
  Host = "https://mysite.com",
  Path = "/userinfo/hi",
  ResponseHandler = HttpResponseHandlers.GetDefaultResponseHandler<UserInfo>();
});
```

