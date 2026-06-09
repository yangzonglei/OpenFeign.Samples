namespace OpenFeign.AOT.Samples.Client.Models;

public sealed record CreateUserRequest(string Name, int Age, string? City = null);
