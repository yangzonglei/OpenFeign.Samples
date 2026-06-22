namespace OpenFeign.Net48.Samples.Client.Models
{
    /// <summary>
    /// 自定义响应结果（status/result/message 格式，用于 StatusResponseResolver）
    /// </summary>
    public class StatusResponseResult<T>
    {
        public string? Status { get; set; }
        public T? Result { get; set; }
        public string? Message { get; set; }
    }
}
