namespace DrawingSpy.Models;

public enum RoundOutcome
{
    /// <summary>Шпиона нашли — все, кроме шпиона, получают по 1 баллу.</summary>
    SpyFound,

    /// <summary>Шпиона определили неверно — только шпион получает 1 балл.</summary>
    SpyNotFound,

    /// <summary>Шпион отгадал слово — шпион получает 2 балла.</summary>
    SpyGuessedWord
}
