namespace OpenFeign.AOT.Samples.Client.Models;

public sealed record UserDto(long Id, string Name, int Age, string? City = null);
