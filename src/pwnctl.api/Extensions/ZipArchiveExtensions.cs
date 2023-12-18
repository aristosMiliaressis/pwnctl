namespace pwnctl.api.Extensions;

using System.IO.Compression;
using pwnctl.infra.Configuration;

public static class ZipArchiveExtensions
{
    public static void AddFolderEntry(this ZipArchive archive, string path)
    {
        var archivePath = path.Replace(EnvironmentVariables.FS_MOUNT_POINT, "");
        
        archive.CreateEntry(archivePath.EndsWith("/") ? archivePath : archivePath + "/");

        foreach (var filePath in Directory.GetFiles(path))
        {
            var fileBytes = File.ReadAllBytes(filePath);

            var fileEntryName = Path.Combine(archivePath, Path.GetFileName(filePath));

            var fileEntry = archive.CreateEntry(fileEntryName, CompressionLevel.Optimal);
            using (var fileEntryStream = fileEntry.Open())
            {
                fileEntryStream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        foreach (var directoryPath in Directory.GetDirectories(path))
        {
            archive.AddFolderEntry(directoryPath);
        }
    }
}
