using System.Text;

using ShellProgressBar;

namespace StellarisDiff;

public sealed class VanillaInfo : IAsyncGetStringable
{
    public required string VanillaPath { get; set; }
    public required string Version { get; set; }

    public async Task<string> ToStringAsync(ProgressBar progressBar, ProgressBarOptions options)
    {
        string[] files = Directory.GetFiles(VanillaPath, "*", SearchOption.AllDirectories);
        using ProgressBarWrapper progress = new(progressBar.Spawn(
            files.Length,
            "Calculating Vanilla File's checksum.",
            options));

        StringBuilder sb = new();
        sb.AppendLine($"vanilla({Version}):");
        int i = 0;
        await foreach ((string Path, string Hash) item in IAsyncGetStringable.GetHash(VanillaPath, files))
        {
            sb.AppendLine($"\t{item.Hash}: {item.Path}");
            i++;
            progress.Report(i);
        }

        progress.Finished();

        return sb.ToString();
    }
}
