using System.Text;
using OpenFeignAot.Samples.Client.Clients;

namespace OpenFeign.AOT.Samples.Client.Fallbacks;

public sealed class DownloadClientFallback : IDownloadClient
{
    private const string FallbackContent = "fallback download content";

    public Task<Stream> DownloadAsync() => Task.FromResult<Stream>(CreateStream());

    public Stream Download() => CreateStream();

    public Task<byte[]> DownloadBytesAsync() => Task.FromResult(Encoding.UTF8.GetBytes(FallbackContent));

    private static MemoryStream CreateStream() => new(Encoding.UTF8.GetBytes(FallbackContent));
}
