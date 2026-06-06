using System.Text.Json;
using DrawingSpy.Models;

namespace DrawingSpy.Services;

/// <summary>Сохраняет и загружает последнюю игру через Preferences.</summary>
public static class GameStorage
{
    private const string Key = "last_saved_game";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    public static bool HasSavedGame => Preferences.ContainsKey(Key);

    public static void Save(GameService game)
    {
        var snapshot = game.CreateSnapshot();
        Preferences.Set(Key, JsonSerializer.Serialize(snapshot, JsonOptions));
    }

    public static SavedGame? Load()
    {
        if (!HasSavedGame)
            return null;

        var json = Preferences.Get(Key, string.Empty);
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            return JsonSerializer.Deserialize<SavedGame>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public static void Clear()
    {
        Preferences.Remove(Key);
    }
}
