namespace OpenFeign.Samples.Api.Models;

public sealed record StatusResponseResult<T>(string Status, T? Result, string Message);
