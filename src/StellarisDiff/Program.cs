using System.Runtime.InteropServices;
using System.Text.Json;

using ShellProgressBar;

using StellarisDiff;


string? gamePath = args.Length is > 0 ? args[0] : null;
string gameDataPath;
while (string.IsNullOrWhiteSpace(gamePath) || !Directory.Exists(gamePath))
{
    Console.Write("Where is your game's path? ");
    gamePath = Console.ReadLine();
}

var document = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
#if NET8_0_OR_GREATER
    // https://learn.microsoft.com/dotnet/core/compatibility/core-libraries/8.0/getfolderpath-unix#linux
    document = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
#endif
    document = Path.Combine(document, ".local", "share");
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
#if !NET8_0_OR_GREATER
    // https://learn.microsoft.com/dotnet/core/compatibility/core-libraries/8.0/getfolderpath-unix#macos
    document = Path.Combine(document, "Documents");
#endif
}

VanillaLauncherSetting setting;
DlcLoad dlcLoad;

await using (var fs = File.OpenRead(Path.Combine(gamePath, "launcher-settings.json")))
    setting = await JsonSerializer.DeserializeAsync<VanillaLauncherSetting>(fs)
        ?? throw new InvalidProgramException("launcher-settings.json");

gameDataPath = setting.GameDataPath.Replace("%USER_DOCUMENTS%", document);
await using (var fs = File.OpenRead(Path.Combine(gameDataPath, "dlc_load.json")))
    dlcLoad = await JsonSerializer.DeserializeAsync<DlcLoad>(fs)
        ?? throw new InvalidProgramException("dlc_load.json");

var hashCalcList = await Task.WhenAll(
    new[]{
        Task.Run<IAsyncGetStringable>(() => new VanillaInfo(){
            VanillaPath = gamePath,
            Version = setting.Version,
        })
    }.Concat(
        dlcLoad.EnabledMods
            .Select(i => Path.Combine(gameDataPath, i))
            .Select(ModInfo.ParseAsync)
    )
);


ProgressBarOptions options = new()
{
    ForegroundColor = ConsoleColor.Magenta,
    BackgroundColor = ConsoleColor.DarkMagenta,
    ForegroundColorDone = ConsoleColor.DarkCyan,
    ProgressCharacter = '─',
    ProgressBarOnBottom = true,
    EnableTaskBarProgress = RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
};
ProgressBarOptions childOptions = new()
{
    ForegroundColor = ConsoleColor.Blue,
    BackgroundColor = ConsoleColor.DarkBlue,
    ForegroundColorDone = ConsoleColor.Green,
    ProgressCharacter = '─',
    CollapseWhenFinished = true,
    ProgressBarOnBottom = true,
    ShowEstimatedDuration = true,
};

DateTime dateTime = DateTime.Now;
using ProgressBar progressBar = new(hashCalcList.Length + 1, $"Calculating {setting.GameId}'s checksum", options);
int count = 0;

var outputs = await Task.WhenAll(hashCalcList.Select(async i =>
{
    try
    {
        progressBar.Tick(count);
        return await i.ToStringAsync(progressBar, childOptions);
    }
    finally
    {
        count++;
        progressBar.Tick(count);
    }
}));

progressBar.Tick(count, "Writing to Log File.");
using var cpbr = new ProgressBarWrapper(progressBar.Spawn(outputs.Length, "Writing to Log File."));
using var output = File.CreateText("diff.log");
await output.WriteLineAsync($"CreateTime: {DateTime.Now:O}");
for (int i = 0; i < outputs.Length; i++)
{
    cpbr.Report(i);
    string? item = outputs[i];
    await output.WriteLineAsync(item);
}
cpbr.Finished();
progressBar.Tick(hashCalcList.Length + 1, "Writing to Log File.");
