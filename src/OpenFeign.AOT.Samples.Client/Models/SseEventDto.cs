namespace OpenFeign.AOT.Samples.Client.Models;

public sealed record SseEventDto(int Index, string Message, bool CompleteSucc = false);
