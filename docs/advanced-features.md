# Advanced features / 高级功能

## Fallback

中文：在 `[FeignClient]` 上指定 fallback 类型。当请求失败、超时或异常时，框架会调用 fallback 中同名方法。

English: Set a fallback type on `[FeignClient]`. When a request fails, times out, or throws, the framework calls the matching fallback method.

```csharp
[FeignClient(name: "demo-api", url: "{DemoApi:BaseUrl}", fallback: typeof(DemoFeignClientFallback))]
public interface IDemoFeignClient
{
    [Get("/api/timeout", timeout: 1000)]
    string TimeoutWithFallback();
}

public sealed class DemoFeignClientFallback : IDemoFeignClient
{
    public string TimeoutWithFallback() => "fallback timeout";
}
```

## Global headers / 全局请求头

中文：实现 `IFeignRequestHeaderProvider` 可以给所有 Feign 请求统一添加请求头。

English: Implement `IFeignRequestHeaderProvider` to add headers to all Feign requests.

```csharp
public sealed class DemoHeaderProvider : IFeignRequestHeaderProvider
{
    public int Order => -100;

    public void Apply(IDictionary<string, string> headers)
    {
        headers.TryAdd("X-Demo-Global", "from-header-provider");
        headers.TryAdd("Authorization", "Bearer demo-token");
    }
}
```

## Custom response resolver / 自定义响应解析

中文：默认 `RawFormat = false` 会按 `{ code, data, msg }` 解析。如果服务返回其他结构，例如 `{ status, result, message }`，可以实现 `IFeignResponseResolver`。

English: By default, `RawFormat = false` resolves `{ code, data, msg }`. If your service returns another shape such as `{ status, result, message }`, implement `IFeignResponseResolver`.

```csharp
[FeignResponse(typeof(StatusResponseResolver))]
[Get("/api/users/{id}/status-result", RawFormat = false)]
Task<UserDto> GetStatusResult([PathVariable("id")] long id);
```

## RawFormat

中文：

- `RawFormat = true`：直接把响应体反序列化成返回类型。
- `RawFormat = false`：先走响应解析器，再把解析出的字段反序列化成返回类型。

English:

- `RawFormat = true`: deserialize the response body directly into the return type.
- `RawFormat = false`: resolve the response through a response resolver first, then deserialize the selected field.

```csharp
[Get("/api/users/{id}/wrapped", RawFormat = false)]
Task<UserDto> GetWrappedData([PathVariable("id")] long id);

[Get("/api/users/{id}/wrapped")]
Task<ResponseResult<UserDto>> GetWrappedRaw([PathVariable("id")] long id);
```

## SSE

中文：当前支持两种 SSE 消费方式。

English: Two SSE consumption styles are supported.

```csharp
[Sse(CompleteField = "CompleteSucc")]
[Get("/api/sse/stream")]
IAsyncEnumerable<SseEventDto> StreamAsAsyncEnumerable();

[Sse(CompleteField = "CompleteSucc")]
[Get("/api/sse/stream")]
ISseStream<SseEventDto> StreamAsSseStream();
```

`IAsyncEnumerable<T>`:

```csharp
await foreach (var item in client.StreamAsAsyncEnumerable())
{
    Console.WriteLine(item);
}
```

`ISseStream<T>`:

```csharp
var stream = client.StreamAsSseStream();
await stream.SubscribeAsync(item =>
{
    Console.WriteLine(item);
    return Task.CompletedTask;
});
```

## Request body types / 请求体类型

中文：`[RequestBody]` 当前支持：

- DTO/object JSON
- `string`
- `byte[]`
- `Stream`
- `HttpContent`

English: `[RequestBody]` currently supports:

- DTO/object JSON
- `string`
- `byte[]`
- `Stream`
- `HttpContent`

```csharp
[Post("/api/body/string")]
Task<object> SendString([RequestBody] string body);

[Post("/api/body/bytes")]
Task<object> SendBytes([RequestBody] byte[] body);

[Post("/api/body/stream")]
Task<object> SendStream([RequestBody] Stream body);

[Post("/api/body/http-content")]
Task<object> SendHttpContent([RequestBody] StringContent content);
```

## Sync and async returns / 同步与异步返回

中文：Feign client 可以声明同步返回，也可以声明 `Task<T>` 异步返回。推荐业务代码优先使用异步方法。

English: A Feign client can use synchronous return types or `Task<T>`. Prefer async methods in application code.

```csharp
[Get("/api/users/{id}")]
UserDto GetByIdSync([PathVariable("id")] long id);

[Get("/api/users/{id}")]
Task<UserDto> GetById([PathVariable("id")] long id);
```
