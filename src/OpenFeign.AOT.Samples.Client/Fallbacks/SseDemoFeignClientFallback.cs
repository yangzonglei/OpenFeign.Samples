using OpenFeignAot.Samples.Client.Clients;
using OpenFeign.AOT.Samples.Client.Models;
using Yzl.Extensions.Http.OpenFeign.Sse;

namespace OpenFeign.AOT.Samples.Client.Fallbacks;

public sealed class SseDemoFeignClientFallback : ISseDemoFeignClient
{
    public async IAsyncEnumerable<SseEventDto> StreamAsAsyncEnumerable()
    {
        yield return new SseEventDto(0, "fallback-sse", true);
        await Task.CompletedTask;
    }

    public ISseStream<SseEventDto> StreamAsSseStream() => new FallbackSseStream();

    private sealed class FallbackSseStream : ISseStream<SseEventDto>
    {
        public bool IsClosed { get; private set; }

        public async Task SubscribeAsync(Func<SseEventDto, Task> onNext, CancellationToken cancellationToken = default)
        {
            await onNext(new SseEventDto(0, "fallback-sse-stream", true));
            IsClosed = true;
        }

        public Task CloseAsync()
        {
            IsClosed = true;
            return Task.CompletedTask;
        }

        public async IAsyncEnumerator<SseEventDto> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            yield return new SseEventDto(0, "fallback-sse-stream", true);
            await Task.CompletedTask;
        }
    }
}
