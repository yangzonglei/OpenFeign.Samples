namespace OpenFeign.Samples.Api.Models;

public sealed record CreateUserRequest(string Name, int Age, string? City = null);
