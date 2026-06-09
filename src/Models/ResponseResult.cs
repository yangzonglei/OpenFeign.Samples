namespace Models;

public sealed record ResponseResult<T>(int Code, T? Data, string Msg);
