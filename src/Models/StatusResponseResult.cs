namespace Models;

public sealed record StatusResponseResult<T>(string Status, T? Result, string Message);
