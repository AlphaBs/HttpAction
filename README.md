# HttpAction
Http client for .NET

----------

[HttpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0) 의 확장 메서드와 여러 유틸 클래스가 있습니다.

## 사용목적

- 하나의 식으로 HTTP API 요청. 요청부터 데이터 처리까지 하나의 객체로 표현
- 간단하게 헤더 추가, URL 쿼리 추가, JSON 데이터 직렬화/역직렬화
- 미리 만들어둔 다양한 응답, 오류 처리 메서드

## 설치
Release 에서 dll 을 다운받아 프로젝트에 직접 참조

## 예제 코드
- [HttpActionTest](https://github.com/AlphaBs/HttpAction/blob/main/HttpActionTest/Program.cs)
- [MojangAPI](https://github.com/CmlLib/MojangAPI)

## 사용법
먼저 응답 데이터를 표현할 모델 클래스를 만들어 줍니다.

```csharp
class UserInfo
{
  public string Name { get; set; }
  public string ID { get; set; }
  public int Age { get; set; }
}
```

### 기본 GET 요청

HTTP 요청 후 응답을 JSON 형태로 파싱하고 UserInfo 클래스로 역직렬화해줍니다.

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

### JSON POST 데이터, 요청 헤더 설정 

```csharp
HttpClient client = new HttpClient();
UserInfo response = await client.SendActionAsync(new HttpAction<UserInfo>
{
  Method = HttpMethod.Post,
  Host = "https://mysite.com",
  Path = "/users",
  RequestHeaders = new HttpHeaderCollection
  {
    { "User-Agent", "askdjfklasdf" },
    { "Accept-Language", "ko-KR" }
  },
  Content = new JsonHttpContent(new
  {
    name = "Hello",
    email = "asdf@gmail.com",
    somedata = 15345
  }
  ResponseHandler = HttpResponseHandlers.GetDefaultResponseHandler<UserInfo>();
});
```

### URL 쿼리 추가, 멀티파트 데이터 전송
`Queries` 속성에 추가한 키-값 컬렉션은 요청 URL 끝에 [URL Query](https://en.wikipedia.org/wiki/Query_string) 형태로 추가됩니다.  
`Content` 속성에는 `StringContent`, `FormUrlEncodedContent`, `Multipartformdatacontent` 등등 HttpContent 관련 클래스를 모두 이용할 수 있습니다.  
[HttpContent 공식 문서](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpcontent?view=net-5.0)

```csharp
HttpClient client = new HttpClient();
UserInfo response = await client.SendActionAsync(new HttpAction<UserInfo>
{
  Method = HttpMethod.Post,
  Host = "https://mysite.com",
  Path = "/users",
  Queries = new HttpQueryCollection
  {
    { "key1", "value1" },
    { "number", "123" }
  }
  Content = new MultipartFormDataContent()
  {
    { new StringContent("value1"), "key_str1" },
    { new StreamContent(File.OpenRead("myFile")), "key_file1" }
  },
  ResponseHandler = HttpResponseHandlers.GetDefaultResponseHandler<UserInfo>();
});
```

### 응답 처리 방법
`Func<HttpResponseMessage, Task<T>>`형식의 `ResponseHandler`속성을 설정하면 됩니다. 비동기 메서드도 설정 가능합니다.  
요청 후 응답이 성공적으로 왔을 때 작동합니다.  

```csharp
HttpClient client = new httpClient();
UserInfo response = await client.SendActionAsync(new HttpAction<UserInfo>
{
  Method = HttpMethod.Get,
  Host = "https://mysite.com",
  Path = "/test",
  ResponseHandler = async (response) =>
  {
    string resStr = await response.Content.ReadAsString();
    UserInfo user = JsonConvert.DeserializeObject<UserInfo>(resStr);
    return user;
  }
}
```

`HttpResponseHandlers` 클래스에는 ResponseHandler 를 위한 다양한 처리 메서드들이 기본으로 들어있습니다.
- GetDefaultResponseHandler<T>()  
  `T`의 형식에 따라 다른 처리기를 반환합니다.  
  `bool`: `GetSuccessCodeResponseHandler()`  
  `int`: `GetStatusCodeResponseHandler()`  
  `string`: `GetStringResponseHandler()`,  
  `Stream`: `GetStreamResponseHandler()`,  
  그 외 타입은 모두 `GetJsonHandler<T>()`가 반환됩니다.  
  
- GetSuccessCodeResponseHandler()  
  응답 StatusCode가 2xx 인 경우에만 true를 반환합니다.  
  
- GetSuccessCodeResponseHandler(int successCode)  
  응답 StatusCode가 `successCode`인 경우에만 true를 반환합니다.  
  
- GetStringResponseHandler()  
  응답을 문자열 그대로 반환합니다.
  
- GetStreamResponseHandler()  
  응답 `Stream` 을 반환합니다.
  
- GetSuccessCodeResponseHandler<T>(T returnObj)  
  응답 StatusCode가 2xx 인 경우에 `returnObj`를 반환합니다.  
  그렇지 않은 경우에는 예외를 발생시킵니다.  
  
- GetJsonHandler<T>()  
  응답 JSON 문자열을 `T`형태로 역직렬화하여 반환합니다.  
  
- GetJsonArrayHandler<T>()  
  응답 JSON 데이터가 배열인 경우에 `T[]` 형식으로 데이터를 역직렬화한 후 반환합니다.  
  예시: `["apple", "banana", "cat"]` => `string[] { "apple", "banana", "cat" }`  
  
## 오류 처리
`Func<HttpResponseMessage, Exception, Task<T>>`형식의 `ErrorHandler` 속성을 설정하면 됩니다.
처리 중 예외가 발생할 때, 응답 코드가 2xx가 아닐 때 작동합니다.  
예외가 발생한 경우 `exception` 인수에 예외 정보가 들어옵니다.

```csharp
HttpClient client = new HttpClient();
UserInfo response = await client.SendActionAsync(new HttpAction<UserInfo>
{
  Method = HttpMethod.Get,
  Host = "https://mysite.com",
  Path = "/test",
  ErrorHandler = (response, ex) =>
  {
    return Task.FromResult(new UserInfo());
  }
}
```

`HttpResponseHandlers`클래스에는 ErrorHandler를 위한 오류 처리 메서드가 기본으로 있습니다.
- GetJsonErrorHandler<T>()  
  응답 JSON 데이터를 `T`형식으로 역직렬화하여 반환합니다.
  
## 입력 유효성 검사
API 메서드를 작성할 때 인수가 유효한 지 검사하기 위해 사용합니다.  
`Func<HttpAction<T>, string>`형식의 `CheckValidation`속성을 설정하면 됩니다.
HTTP 요청을 보내기 전에 실행되며, 메서드가 null 이 아닌 값을 반환하면 `ArgumentException`을 발생시킵니다.

```csharp
public UserInfo GetUserInfo(string id) =>
  client.SendActionAsync(new HttpActioin<UserInfo>
  {
    Method = HttpMethod.Get,
    Host = "https://mysite.com",
    Path = $"/users/{id}",
    CheckValidation = (h) => 
    {
      if (string.IsNullOrEmpty(id) || id.Length > 12)
        return nameof(id);
      else
        return null;
    }
  });
  
// GetUserInfo(null) => ArgumentException("id") 발생!
// GetUserInfo("Hello") => UserInfo 객체 반환
```
