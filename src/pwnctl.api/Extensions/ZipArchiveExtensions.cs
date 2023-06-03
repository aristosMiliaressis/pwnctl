namespace pwnctl.api.Extensions;

using System.IO.Compression;
using pwnctl.infra.Configuration;

public static class ZipArchiveExtensions
{
    public static void AddFolderEntry(this ZipArchive archive, string path)
    {
        foreach (var file in Directory.GetFiles(path))
        {
            var fileBytes = File.ReadAllBytes(file);

            var fileInArchive = archive.CreateEntry(file.Replace(EnvironmentVariables.INSTALL_PATH, "").Replace("/", "_"), CompressionLevel.Fastest);
            using (var entryStream = fileInArchive.Open())
            {
                entryStream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        foreach (var directory in Directory.GetDirectories(path))
        {
            archive.AddFolderEntry(Path.Combine(path, directory));
        }
    }
}
