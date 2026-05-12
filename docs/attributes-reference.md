# Attributes reference / 特性参考

## HTTP 方法特性 / HTTP method attributes

| Attribute | Method | Example |
| --- | --- | --- |
| `[Get]` | GET | `[Get("/api/users/{id}")]` |
| `[Post]` | POST | `[Post("/api/users")]` |
| `[Put]` | PUT | `[Put("/api/users/{id}")]` |
| `[Patch]` | PATCH | `[Patch("/api/users/{id}")]` |
| `[Delete]` | DELETE | `[Delete("/api/users/{id}")]` |
| `[Head]` | HEAD | `[Head("/api/methods/head")]` |
| `[Options]` | OPTIONS | `[Options("/api/methods/options")]` |
| `[Trace]` | TRACE | `[Trace("/api/methods/trace")]` |

中文：所有方法特性都继承自 `HttpMethodAttribute`，可设置 `RequestUri`、`ContentType`、`RawFormat`、`Timeout`。

English: All method attributes inherit from `HttpMethodAttribute` and support `RequestUri`, `ContentType`, `RawFormat`, and `Timeout`.

```csharp
[Get("/api/timeout", timeout: 1000)]
string TimeoutWithFallback();

[Get("/api/users/{id}/wrapped", RawFormat = false)]
Task<UserDto> GetWrappedData([PathVariable("id")] long id);
```

## FeignClient

中文：`[FeignClient]` 用来声明一个接口是 Feign 客户端。

English: `[FeignClient]` marks an interface as a Feign client.

```csharp
[FeignClient(name: "demo-api", url: "{DemoApi:BaseUrl}", fallback: typeof(DemoFeignClientFallback), timeout: 5000)]
public interface IDemoFeignClient
{
}
```

参数 / Parameters:

- `name`: 客户端名称 / client name
- `url`: 基础地址，支持配置占位符 / base URL, supports configuration placeholders
- `fallback`: 请求失败时调用的 fallback 类型 / fallback type used when a request fails
- `timeout`: 客户端默认超时时间，单位毫秒 / default client timeout in milliseconds

## 参数绑定 / Parameter binding

### PathVariable

```csharp
[Get("/api/users/{id}")]
Task<UserDto> GetById([PathVariable("id")] long id);
```

中文：替换 URL path 中的 `{id}`。

English: Replaces `{id}` in the URL path.

### RequestParam

```csharp
[Get("/api/users/query")]
Task<object> Query([RequestParam("id")] long id, [RequestParam("name")] string name);
```

中文：生成 query string 参数。

English: Adds query string parameters.

### QueryMap

```csharp
[Get("/api/users/map")]
Task<object> QueryMap([QueryMap] Dictionary<string, string> values);
```

中文：把字典或对象展开为 query string。

English: Expands a dictionary or object into query string parameters.

### RequestHeader

```csharp
[Get("/api/headers")]
Task<object> Headers([RequestHeader("X-Token")] string token, [RequestHeader] string requestSource = "manual-header");
```

中文：添加请求头。未显式指定名称时，使用参数名。

English: Adds request headers. If no name is specified, the parameter name is used.

### RequestBody

```csharp
[Post("/api/users")]
Task<UserDto> Create([RequestBody] CreateUserRequest request);
```

中文：发送请求体。当前支持 DTO/object JSON、`string`、`byte[]`、`Stream`、`HttpContent`。

English: Sends a request body. The current implementation supports DTO/object JSON, `string`, `byte[]`, `Stream`, and `HttpContent`.

## RequestMapping

中文：`RequestMapping` 是通用请求映射特性，适合需要显式指定 HTTP method 字符串的场景。

English: `RequestMapping` is a generic mapping attribute for scenarios where you want to specify the HTTP method string directly.

```csharp
[RequestMapping("GET", "/api/users/{id}")]
Task<UserDto> GetByRequestMapping([PathVariable("id")] long id);
```

## Sse

```csharp
[Sse(CompleteField = "CompleteSucc")]
[Get("/api/sse/stream")]
IAsyncEnumerable<SseEventDto> StreamAsAsyncEnumerable();

[Sse(CompleteField = "CompleteSucc")]
[Get("/api/sse/stream")]
ISseStream<SseEventDto> StreamAsSseStream();
```

中文：`CompleteField` 指定事件对象中表示流结束的字段。

English: `CompleteField` specifies the event property that indicates stream completion.

## FeignResponse

```csharp
[FeignResponse(typeof(StatusResponseResolver))]
[Get("/api/users/{id}/status-result", RawFormat = false)]
Task<UserDto> GetStatusResult([PathVariable("id")] long id);
```

中文：用于为接口或方法指定自定义响应解析器。

English: Specifies a custom response resolver for an interface or method.

## 暂不支持 / Not supported yet

中文：当前版本暂不支持以下参数绑定：

- cookie parameter
- form-urlencoded
- multipart/form-data 或文件上传

English: The current version does not implement these bindings yet:

- cookie parameter
- form-url-encoded requests
- multipart/form-data or file upload
