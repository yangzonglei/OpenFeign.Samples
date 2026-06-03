using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace OpenFeign.Samples.Api.Controllers;

[Route("api/download")]
public class DownloadController : ControllerBase
{
    /// <summary>
    ///  测试文件下载
    /// </summary>
    /// <returns></returns>
    [HttpGet("files/abc.doc")]
    public FileContentResult DownloadFile()
    {
        var bytes = Encoding.UTF8.GetBytes("OpenFeign file download test content.");
        return File(bytes, "application/msword", "abc.doc");
    }
}