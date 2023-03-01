namespace pwnctl.api.Extensions;

using System.IO.Compression;
using pwnctl.infra.Configuration;

public static class ZipArchiveExtensions
{
    public static void AddFolderEntry(this ZipArchive archive, string path)
    {
        foreach (var file in Directory.GetFiles(path))
        {
            archive.CreateEntryFromFile(file, file.Replace(EnvironmentVariables.InstallPath+"/", ""));
        }

        foreach (var directory in Directory.GetDirectories(path))
        {
            archive.AddFolderEntry(Path.Combine(path, directory));
        }
    }
}