namespace OpenFeign.Samples.Api.Models;

public sealed record SseEventDto(int Index, string Message, bool CompleteSucc = false);
