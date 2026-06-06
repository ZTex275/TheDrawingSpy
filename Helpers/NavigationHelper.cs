namespace DrawingSpy.Helpers;

public static class NavigationHelper
{
    /// <summary>
    /// Открывает таблицу итогов раунда, убирая экраны раздачи и выбора исхода из стека.
    /// </summary>
    public static async Task GoToRoundScoreboardAsync(INavigation navigation)
    {
        var scoreboard = new Pages.ScoreboardPage(roundSummary: true);
        navigation.InsertPageBefore(scoreboard, navigation.NavigationStack[0]);
        await navigation.PopToRootAsync();
    }

    /// <summary>
    /// Переходит к следующему раунду, не возвращаясь на экран настройки.
    /// </summary>
    public static async Task GoToNextRoundAsync(INavigation navigation)
    {
        var nextRound = new Pages.RoundIntroPage();
        navigation.InsertPageBefore(nextRound, navigation.NavigationStack[0]);
        await navigation.PopToRootAsync();
    }

    /// <summary>
    /// Сохраняет текущие настройки и возвращает на экран новой игры.
    /// </summary>
    public static void GoToNewGame()
    {
        Services.GameStorage.Save(App.Game);

        Application.Current!.Windows[0].Page = new NavigationPage(new Pages.SetupPage())
        {
            BarBackgroundColor = Color.FromArgb("#512BD4"),
            BarTextColor = Colors.White
        };
    }

    /// <summary>
    /// Запрашивает подтверждение и начинает новую игру.
    /// </summary>
    public static async Task<bool> ConfirmAndStartNewGameAsync(Page page)
    {
        bool confirmed = await ConfirmDialogHelper.ShowNewGameConfirmAsync(page);

        if (!confirmed)
            return false;

        GoToNewGame();
        return true;
    }
}
