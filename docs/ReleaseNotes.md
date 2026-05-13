# Yzl.Extensions.Http.OpenFeign 更新日志

## v0.1.16 [2026/05/11]
1. 修复 GET 请求复杂类型参数序列化逻辑，避免复杂对象作为查询参数时序列化不正确
2. 优化序列化器接口与代理工厂实现，减少无用日志输出并简化 FeignClient 代理创建流程
3. 优化 SSE 处理性能，缓存 Complete 字段 getter，减少流式解析过程中的反射开销
4. 重构 PathVariablePayload，预先生成占位符，减少运行时字符串分配，降低 GC 压力
5. 重构路径变量处理逻辑，支持自定义占位符格式
6. 重构参数解析器接口和实现，引入参数解析器注册表，提升参数解析性能与扩展性
7. 支持单文件部署
8. 依赖 Yzl.Extensions.Common v0.1.4

## v0.1.15 [2026/05/03]
1. 新增IFeignResponseResolver接口和FeignResponseAttribute，允许为Feign客户端接口或方法指定自定义响应解析逻辑

FeignResponseAttribute
```csharp
[FeignResponse(typeof(ResultFeignResponseResolver))]
    [Get("/api/test/users/{id}/getbyid3", RawFormat = false)]
    UserDto GetByIdSync4([PathVariable("id")] long id);
```
IFeignResponseResolver
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


## v0.1.14 
1. 修复序列化问题

## v0.1.13 [2026/04/20]
1. 添加服务器发送事件(SSE)支持
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
2. 重构FeignClient拦截器，简化创建代理对象参数

## v0.1.12 [2026/04/19]
1. 支持为单个接口方法配置超时时间。例如：`[Get("/api/test/timeout",timeout:7000)]`

## v0.1.11 [2026/04/16]
1. 为OpenFeign添加详细的调试日志记录功能。当项目日志级别设置为Debug时，会打印出完整的HTTP请求头、请求体和响应内容，便于开发和问题排查。

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

## v0.1.10 [2026/04/05]
1. FeignOptions 增加SerializerType配置，支持用户自定义序列化器，默认使用NewtonsoftJson
```csharp
builder.Services.AddFeignStarter(builder.Configuration, options => { 
    options.SerializerType = typeof(SystemTextJsonFeignSerializer); 
});
```

## v0.1.9
.....