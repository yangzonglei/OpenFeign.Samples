namespace OpenFeign.Samples.Api.Models;

public sealed record UpdateUserRequest(string Name, int Age, string? City = null);
