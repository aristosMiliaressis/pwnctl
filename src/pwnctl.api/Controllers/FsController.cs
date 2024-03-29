namespace pwnctl.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using pwnctl.infra.Configuration;
using System.IO.Compression;
using pwnctl.api.Extensions;

[ApiController]
[Route("[controller]")]
public sealed class FsController : ControllerBase
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

        var directoryListing = Directory.GetDirectories(filePath)
                                .Concat(Directory.GetFiles(filePath));

        return Ok(directoryListing.Select(f => f.Replace(EnvironmentVariables.FS_MOUNT_POINT+path, "")));
    }

    [HttpGet("download")]
    public ActionResult Download([FromQuery] string path)
    {
        string filePath = FullPath(path);

        if (Directory.Exists(filePath))
        {
            var zipFilename = filePath.Replace(EnvironmentVariables.FS_MOUNT_POINT, "").Replace("/", "_")+".zip";

            using (var memoryStream = new MemoryStream())
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                archive.AddFolderEntry(filePath);

                return File(memoryStream.ToArray(), "application/zip", zipFilename);
            }
        }

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        string contentType = GetFileContentType(filePath);

        var fileStream = System.IO.File.OpenRead(filePath);

        return File(fileStream, contentType, Path.GetFileName(filePath));
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

    private string FullPath(string path) => EnvironmentVariables.FS_MOUNT_POINT + (path.StartsWith("/") ? "" : "/") + path;

    private string GetFileContentType(string filePath)
    {
        var provider = new FileExtensionContentTypeProvider();

        return provider.TryGetContentType(filePath, out string? contentType)
                        ? contentType
                        : "application/octet-stream";
    }
}
