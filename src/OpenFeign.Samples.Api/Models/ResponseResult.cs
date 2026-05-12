namespace OpenFeign.Samples.Api.Models;

public sealed record ResponseResult<T>(int Code, T? Data, string Msg);
