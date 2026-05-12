using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenFeign.Samples.Api.Models;

namespace OpenFeign.Samples.Api.Controllers;

[ApiController]
[Route("api/sse")]
public sealed class SseController : ControllerBase
{
    [HttpGet("stream")]
    public async Task Stream(CancellationToken cancellationToken)
    {
        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";

        for (var i = 1; i <= 3; i++)
        {
            var data = JsonSerializer.Serialize(new SseEventDto(i, $"SSE message {i}"));
            await Response.WriteAsync($"data: {data}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
            await Task.Delay(200, cancellationToken);
        }

        var complete = JsonSerializer.Serialize(new SseEventDto(4, "complete", true));
        await Response.WriteAsync($"data: {complete}\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }
}
