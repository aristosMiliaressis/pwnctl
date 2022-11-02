namespace pwnctl.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using pwnwrk.infra.Configuration;

[ApiController]
[Route("[controller]")]
public class FsController : ControllerBase
{
    private readonly ILogger<FsController> _logger;

    public FsController(ILogger<FsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<string[]> DirectoryListing([FromQuery] string path)
    {
        string filePath = FullPath(path);

        if (!Directory.Exists(filePath))
            return NotFound();

        var directoryListing = Directory.GetDirectories(filePath).ToList();
        directoryListing.AddRange(Directory.GetFiles(filePath));

        return Ok(directoryListing.Select(f => f.Replace(EnvironmentVariables.InstallPath, "")));
    }

    [HttpGet("download")]
    public ActionResult Download([FromQuery] string path)
    {
        string filePath = FullPath(path);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        string contentType = GetFileContentType(filePath);

        var bytes = System.IO.File.ReadAllBytes(filePath);
        var content = new System.IO.MemoryStream(bytes);

        return File(content, contentType, Path.GetFileName(filePath));
    }

    [HttpPut("upload")]
    public async Task<ActionResult> Upload([FromQuery] string path)
    {
        string filePath = FullPath(path);

        byte[] buffer = new byte[1024];
        int len;

        using (MemoryStream stream = new())
        {
            while ((len = await Request.Body.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, len);
            }

            System.IO.File.WriteAllBytes(filePath, stream.ToArray());
        }

        return Ok();
    }

    [HttpDelete("delete")]
    public ActionResult Delete([FromQuery] string path)
    {
        string filePath = FullPath(path);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);

            return Ok();
        }

        if (Directory.Exists(filePath))
        {
            Directory.Delete(filePath);

            return Ok();
        }

        return NotFound();
    }

    [HttpPut("create")]
    public ActionResult Create([FromQuery] string path)
    {
        string filePath = FullPath(path);

        if (System.IO.File.Exists(filePath) || Directory.Exists(filePath))
            return BadRequest();

        Directory.CreateDirectory(filePath);

        return Ok();
    }

    private string FullPath(string path) => EnvironmentVariables.InstallPath + (path.StartsWith("/") ? "" : "/") + path;

    private string GetFileContentType(string filePath)
    {
        var provider = new FileExtensionContentTypeProvider();

        return provider.TryGetContentType(filePath, out string contentType)
                        ? contentType
                        : "application/octet-stream";
    }
}
