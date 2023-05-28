using System.Security.Cryptography;

using ShellProgressBar;

namespace StellarisDiff;

public interface IAsyncGetStringable
{
    Task<string> ToStringAsync(ProgressBar progressBar, ProgressBarOptions options);

    static async IAsyncEnumerable<(string Path, string Hash)> GetHash(string path, string[] files)
    {
        for (int i = 0; i < files.Length; i++)
        {
            await using FileStream fs = File.OpenRead(files[i]);
            byte[] md5 = await MD5.HashDataAsync(fs);
            yield return (
                Path: Path.GetRelativePath(path, files[i]).Replace('/', '\\'),
                Hash: BitConverter.ToString(md5).Replace("-", "")
            );
        }
    }
}
