using System.Security.Cryptography;
using System.Text;

using ShellProgressBar;

namespace StellarisDiff;

public sealed class ModInfo : IAsyncGetStringable
{
    private ModInfo()
    {
    }

    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SupportVersion { get; set; } = string.Empty;
    public string ModPath { get; set; } = string.Empty;
    public string RemoteFileId { get; set; } = string.Empty;

    public static async Task<IAsyncGetStringable> ParseAsync(string path)
    {
        string[] lines = await File.ReadAllLinesAsync(path);
        ModInfo result = new();
        foreach (string line in lines)
        {
            string[] tmp = line.Split('=');
            if (tmp.Length < 2)
            {
                continue;
            }

            switch (tmp[0])
            {
                case "version":
                    result.Version = tmp[1].Trim('"');
                    break;
                case "name":
                    result.Name = tmp[1].Trim('"');
                    break;
                case "supported_version":
                    result.SupportVersion = tmp[1].Trim('"');
                    break;
                case "archive":
                    result.ModPath = Path.GetDirectoryName(tmp[1].Trim('"'))!;
                    break;
                case "path":
                    result.ModPath = tmp[1].Trim('"');
                    break;
                case "remote_file_id":
                    result.RemoteFileId = tmp[1].Trim('"');
                    break;
                default:
                    break;
            }
        }
        return result;
    }


    public async Task<string> ToStringAsync(ProgressBar progressBar, ProgressBarOptions options)
    {
        string[] files = Directory.GetFiles(ModPath, "*", SearchOption.AllDirectories);
        using ProgressBarWrapper progress = new(progressBar.Spawn(
            files.Length,
            $"Calculating Mod \"{Name}[{RemoteFileId}]\" File's checksum.",
            options));

        StringBuilder sb = new();
        _ = sb.AppendLine($"{Name}[{RemoteFileId}]({Version}):");
        int i = 0;
        await foreach ((string Path, string Hash) item in IAsyncGetStringable.GetHash(ModPath, files))
        {
            sb.AppendLine($"\t{item.Hash}: {item.Path}");
            i++;
            progress.Report(i);
        }

        progress.Finished();

        return sb.ToString();
    }
}
