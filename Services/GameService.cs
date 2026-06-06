using DrawingSpy.Models;

namespace DrawingSpy.Services;

/// <summary>
/// Хранит всё состояние игры: список игроков, количество раундов,
/// текущий раунд, выбранного шпиона, загаданное слово и набранные баллы.
/// Регистрируется как singleton, поэтому состояние общее для всех страниц.
/// </summary>
public class GameService
{
    private readonly Random _rng = new();

    public List<Player> Players { get; } = new();

    public int TotalRounds { get; private set; }

    /// <summary>Номер текущего раунда, начиная с 1.</summary>
    public int CurrentRound { get; private set; }

    /// <summary>Индекс игрока-шпиона в текущем раунде.</summary>
    public int SpyIndex { get; private set; }

    /// <summary>Загаданное слово текущего раунда.</summary>
    public string CurrentWord { get; private set; } = string.Empty;

    /// <summary>Баллы по раундам: RoundScores[номерРаунда-1][индексИгрока].</summary>
    public List<int[]> RoundScores { get; } = new();

    /// <summary>Индекс шпиона по раундам (для таблицы).</summary>
    public List<int> SpyPerRound { get; } = new();

    public bool IsGameFinished => CurrentRound >= TotalRounds && RoundScores.Count >= TotalRounds;

    /// <summary>Создаёт новую игру с именами по умолчанию.</summary>
    public void NewGame(int playerCount, int rounds)
    {
        Players.Clear();
        for (int i = 0; i < playerCount; i++)
        {
            Players.Add(new Player { Index = i, Name = $"Игрок {i + 1}" });
        }

        TotalRounds = rounds;
        CurrentRound = 0;
        SpyIndex = -1;
        CurrentWord = string.Empty;
        RoundScores.Clear();
        SpyPerRound.Clear();
    }

    /// <summary>Гарантирует корректное имя игрока (не пустое).</summary>
    public void SetPlayerName(int index, string? name)
    {
        if (index < 0 || index >= Players.Count)
            return;

        var clean = string.IsNullOrWhiteSpace(name) ? $"Игрок {index + 1}" : name.Trim();
        Players[index].Name = clean;
    }

    /// <summary>Начинает следующий раунд: выбирает шпиона и слово случайным образом.</summary>
    public void StartNextRound()
    {
        CurrentRound++;
        SpyIndex = _rng.Next(Players.Count);
        CurrentWord = WordList.All[_rng.Next(WordList.All.Length)];
    }

    public bool IsSpy(int playerIndex) => playerIndex == SpyIndex;

    /// <summary>Применяет исход раунда и начисляет баллы.</summary>
    public void ApplyOutcome(RoundOutcome outcome)
    {
        var scores = new int[Players.Count];

        switch (outcome)
        {
            case RoundOutcome.SpyFound:
                for (int i = 0; i < Players.Count; i++)
                    scores[i] = i == SpyIndex ? 0 : 1;
                break;

            case RoundOutcome.SpyNotFound:
                scores[SpyIndex] = 1;
                break;

            case RoundOutcome.SpyGuessedWord:
                scores[SpyIndex] = 2;
                break;
        }

        RoundScores.Add(scores);
        SpyPerRound.Add(SpyIndex);
    }

    /// <summary>Сумма баллов игрока за все сыгранные раунды.</summary>
    public int TotalScore(int playerIndex)
    {
        int sum = 0;
        foreach (var round in RoundScores)
        {
            if (playerIndex < round.Length)
                sum += round[playerIndex];
        }
        return sum;
    }

    /// <summary>Создаёт снимок текущего состояния для сохранения.</summary>
    public SavedGame CreateSnapshot()
    {
        return new SavedGame
        {
            TotalRounds = TotalRounds,
            CurrentRound = CurrentRound,
            PlayerNames = Players.Select(p => p.Name).ToList(),
            RoundScores = RoundScores.Select(scores => scores.ToArray()).ToList(),
            SpyPerRound = SpyPerRound.ToList()
        };
    }

    /// <summary>Восстанавливает игру из сохранённого снимка.</summary>
    public void RestoreFromSaved(SavedGame saved)
    {
        NewGame(saved.PlayerNames.Count, saved.TotalRounds);

        for (int i = 0; i < saved.PlayerNames.Count; i++)
            SetPlayerName(i, saved.PlayerNames[i]);

        CurrentRound = saved.CurrentRound;
        RoundScores.Clear();
        foreach (var scores in saved.RoundScores)
            RoundScores.Add(scores);

        SpyPerRound.Clear();
        SpyPerRound.AddRange(saved.SpyPerRound);
    }
}
