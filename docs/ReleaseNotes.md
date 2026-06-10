# Yzl.Extensions.Http.OpenFeign 更新日志

## v0.1.18 [2026/06/10]

### 更新内容（本次无新增功能，只做框架升级）

1. 新增 `Yzl.Extensions.Http.OpenFeign.Abstractions` 公共抽象项目，用于承载运行时代理和 AOT OpenFeign 共用的契约、特性、异常、序列化接口、请求头接口以及 SSE 类型
2. 将 `FeignClient`、请求映射、参数绑定、SSE 等公共特性从运行时代理包与 AOT 包中抽离到 Abstractions 项目，避免两套实现重复定义基础类型
3. 将 `IFeignClientRegistration`、`IOrdered`、`IFeignSerializer`、`IFeignRequestHeaderProvider`、`FeignClientException`、`ISseStream` 等公共接口和类型统一迁移到 Abstractions 项目，便于后续扩展和复用
4. 为运行时代理包和 AOT 包增加对 Abstractions 项目的引用，保持两种 OpenFeign 实现使用同一套公共 API
5. 调整 AOT 源生成器和示例项目的引用与命名空间，适配公共抽象层拆分后的项目结构

---

## v0.1.17 [2026/06/03]

### 更新内容

1. 新增文件下载响应支持，Feign 接口返回 `Stream` 或 `byte[]` 时直接读取二进制内容，避免下载内容被当作字符串或 JSON 解析
2. 优化 `Stream` 下载场景的 HTTP 响应生命周期管理，调用方释放返回流时同步释放底层 `HttpResponseMessage`，避免连接资源泄漏
3. 优化文件下载调试日志，下载响应不再读取和输出响应体，避免日志记录提前消费流或将二进制内容加载到内存
4. 新增文件下载测试接口与测试端点，覆盖异步 `Stream`、同步 `Stream` 和 `byte[]` 下载调用方式

---

## v0.1.16 [2026/05/11]

### 更新内容

1. 修复 GET 请求复杂类型参数序列化逻辑，避免复杂对象作为查询参数时序列化不正确
2. 优化序列化器接口与代理工厂实现，减少无用日志输出并简化 FeignClient 代理创建流程
3. 优化 SSE 处理性能，缓存 Complete 字段 getter，减少流式解析过程中的反射开销
4. 重构 PathVariablePayload，预先生成占位符，减少运行时字符串分配，降低 GC 压力
5. 重构路径变量处理逻辑，支持自定义占位符格式
6. 重构参数解析器接口和实现，引入参数解析器注册表，提升参数解析性能与扩展性
7. 支持单文件部署
8. 依赖 Yzl.Extensions.Common v0.1.4

---

## v0.1.15 [2026/05/03]

### 更新内容

1. 新增IFeignResponseResolver接口和FeignResponseAttribute，允许为Feign客户端接口或方法指定自定义响应解析逻辑

### 示例

#### FeignResponseAttribute

```csharp
[FeignResponse(typeof(ResultFeignResponseResolver))]
[Get("/api/test/users/{id}/getbyid3", RawFormat = false)]
UserDto GetByIdSync4([PathVariable("id")] long id);
```

#### IFeignResponseResolver

```csharp
using System.Text.Json;
using Yzl.Extensions.Http.OpenFeign.Execution;
using Yzl.Extensions.Http.OpenFeign.Execution.ResponseResolver;
using Yzl.Extensions.Http.OpenFeign.Serializer;

namespace Test.Yzl.Extensions.Http.OpenFeign.FeignImpl;

/// <summary>
/// {"status": 200, "result": "xxx" , "message": "xxx"}
/// </summary>
public sealed class ResultFeignResponseResolver : IFeignResponseResolver
{
    public int Order => 1;

    public bool IsGlobal => false;

    public object? Resolve(
        JsonElement root,
        Type resultType,
        FeignRequestContext context,
        IFeignSerializer serializer)
    {
        if (!root.TryGetProperty("status", out var statusElement) ||
            !statusElement.TryGetInt32(out var status) ||
            status != 200)
        {
            return null;
        }

        if (!root.TryGetProperty("result", out var resultElement))
            return null;

        if (resultElement.ValueKind == JsonValueKind.Null)
            return null;

        if (resultType == typeof(string))
        {
            return resultElement.ValueKind == JsonValueKind.String
                ? resultElement.GetString()
                : resultElement.GetRawText();
        }

        if (resultType == typeof(object))
            return resultElement.Clone();

        return serializer.Deserialize(resultElement, resultType);
    }
}
```

---

## v0.1.14

### 更新内容

1. 修复序列化问题

---

## v0.1.13 [2026/04/20]

### 更新内容

1. 添加服务器发送事件(SSE)支持

### 示例

```csharp
[FeignClient(name: "test", url: "http://localhost:17007", fallback: typeof(TestApiFeignClientFallback), timeout: 5000)]
public interface ITestApiFeignClient
{
    [Get("/api/test/timeout", timeout: 7000)]
    string TimeOut([RequestHeader] string a = "123", [RequestHeader(name: "user-token", Encoded = true)] string utk = "你好");

    [Sse(CompleteField = "completeSucc")]
    [Get("/api/RandomChinese/stream")]
    IAsyncEnumerable<RandomChineseSseDto> RandomChinese();
    
    [Sse(CompleteField = "completeSucc")]
    [Get("/api/RandomChinese/stream")]
    ISseStream<RandomChineseSseDto> RandomChinese2();
}

public class TestController(ITestApiFeignClient client) : Controller
{

    [HttpGet("stream-proxy")]
    public async Task StreamProxy(CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");

        await foreach (var item in client.RandomChinese().WithCancellation(cancellationToken))
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);

            await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }

    [HttpGet("stream-proxy1")]
    public async Task StreamProxy1(CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");

        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        await foreach (var item in client.RandomChinese2().WithCancellation(cancellationToken))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(item, options);

            await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
```

### 其他

2. 重构FeignClient拦截器，简化创建代理对象参数

---

## v0.1.12 [2026/04/19]

### 更新内容

1. 支持为单个接口方法配置超时时间。例如：`[Get("/api/test/timeout",timeout:7000)]`

---

## v0.1.11 [2026/04/16]

### 更新内容

1. 为OpenFeign添加详细的调试日志记录功能。当项目日志级别设置为Debug时，会打印出完整的HTTP请求头、请求体和响应内容，便于开发和问题排查。

### 示例

请求接口调试日志如下：

```text
OpenFeignResponse
method: GetTagQuota
uri: http://xxx
httpStatus: OK
===== Request Headers =====
trace-id: cd1e6b1023d6a850886c5e065319aed2
User-Agent: Net.OpenFeign/1.0
traceparent: 00-cd1e6b1023d6a850886c5e065319aed2-5c982b089e1d60c4-00
===== Request Body =====
(null)
===== Response =====
{"code":"0","data":{"canCreate":true,"current":0,"limit":5,"remaining":5},"msg":"成功"}
```

---

## v0.1.10 [2026/04/05]

### 更新内容

1. FeignOptions 增加SerializerType配置，支持用户自定义序列化器，默认使用NewtonsoftJson

### 示例

```csharp
builder.Services.AddFeignStarter(builder.Configuration, options => { 
    options.SerializerType = typeof(SystemTextJsonFeignSerializer); 
});
```

---

## v0.1.9

.....