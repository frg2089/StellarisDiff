using System.Text.Json.Serialization;

namespace StellarisDiff;

// Love you json2csharp.com

public record DlcLoad(
    [property: JsonPropertyName("enabled_mods")] IReadOnlyList<string> EnabledMods,
    [property: JsonPropertyName("disabled_dlcs")] IReadOnlyList<object> DisabledDlcs
);

public record AlternativeExecutable(
    [property: JsonPropertyName("exeArgs")] IReadOnlyList<string> ExeArgs,
    [property: JsonPropertyName("exePath")] string ExePath,
    [property: JsonPropertyName("label")] Label Label,
    [property: JsonPropertyName("visibleIn")] IReadOnlyList<string> VisibleIn
);

public record Label(
    [property: JsonPropertyName("de")] string De,
    [property: JsonPropertyName("en")] string En,
    [property: JsonPropertyName("es")] string Es,
    [property: JsonPropertyName("fr")] string Fr,
    [property: JsonPropertyName("ja")] string Ja,
    [property: JsonPropertyName("ko")] string Ko,
    [property: JsonPropertyName("pl")] string Pl,
    [property: JsonPropertyName("pt")] string Pt,
    [property: JsonPropertyName("ru")] string Ru,
    [property: JsonPropertyName("tr")] string Tr,
    [property: JsonPropertyName("zh-hans")] string ZhHans,
    [property: JsonPropertyName("zh-hant")] string ZhHant
);

public record VanillaLauncherSetting(
    [property: JsonPropertyName("alternativeExecutables")] IReadOnlyList<AlternativeExecutable> AlternativeExecutables,
    [property: JsonPropertyName("browserDlcUrl")] string BrowserDlcUrl,
    [property: JsonPropertyName("distPlatform")] string DistPlatform,
    [property: JsonPropertyName("exeArgs")] IReadOnlyList<string> ExeArgs,
    [property: JsonPropertyName("exePath")] string ExePath,
    [property: JsonPropertyName("gameDataPath")] string GameDataPath,
    [property: JsonPropertyName("gameId")] string GameId,
    [property: JsonPropertyName("ingameSettingsLayoutPath")] string IngameSettingsLayoutPath,
    [property: JsonPropertyName("rawVersion")] string RawVersion,
    [property: JsonPropertyName("themeFile")] string ThemeFile,
    [property: JsonPropertyName("version")] string Version
);