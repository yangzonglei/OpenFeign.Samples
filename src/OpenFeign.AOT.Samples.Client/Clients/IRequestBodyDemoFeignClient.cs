using System.Text;
using OpenFeign.AOT.Samples.Client.Fallbacks;
using OpenFeign.AOT.Samples.Client.Models;
using Yzl.Extensions.Http.OpenFeign.Attributes;
using Yzl.Extensions.Http.OpenFeign.Attributes.Methods;

namespace OpenFeignAot.Samples.Client.Clients;

[FeignClient(name: "demo-api-aot-body", url: "{DemoApi:BaseUrl}", fallback: typeof(RequestBodyDemoFeignClientFallback), timeout: 5000)]
public interface IRequestBodyDemoFeignClient
{
    [Post("/api/users")]
    Task<UserDto> SendObject([RequestBody] CreateUserRequest payload);

    [Post("/api/body/string")]
    Task<object> SendString([RequestBody] string body);

    [Post("/api/body/bytes")]
    Task<object> SendBytes([RequestBody] byte[] body);

    [Post("/api/body/stream")]
    Task<object> SendStream([RequestBody] Stream body);

    [Post("/api/body/http-content")]
    Task<object> SendHttpContent([RequestBody] StringContent content);

    static StringContent CreateJsonContent(string json) => new(json, Encoding.UTF8, "application/json");
}
