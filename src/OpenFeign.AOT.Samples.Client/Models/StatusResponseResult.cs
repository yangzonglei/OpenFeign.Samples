namespace OpenFeign.AOT.Samples.Client.Models;

public sealed record StatusResponseResult<T>(string Status, T? Result, string Message);
