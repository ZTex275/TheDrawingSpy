namespace DrawingSpy.Helpers;

public static class NavigationHelper
{
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
    /// Сбрасывает навигацию на экран новой игры.
    /// </summary>
    public static void GoToNewGame()
    {
        Application.Current!.Windows[0].Page = new NavigationPage(new Pages.SetupPage())
        {
            BarBackgroundColor = Color.FromArgb("#512BD4"),
            BarTextColor = Colors.White
        };
    }
}
