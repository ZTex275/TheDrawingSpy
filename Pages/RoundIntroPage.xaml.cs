using DrawingSpy.Helpers;

namespace DrawingSpy.Pages;

public partial class RoundIntroPage : ContentPage
{
    public RoundIntroPage()
    {
        InitializeComponent();
        var game = App.Game;
        int next = game.CurrentRound + 1;
        RoundLabel.Text = $"Раунд {next}";
        RoundSubLabel.Text = $"из {game.TotalRounds}";
    }

    private async void OnStartRoundClicked(object? sender, EventArgs e)
    {
        App.Game.StartNextRound();
        await Navigation.PushAsync(new RevealPage());
    }

    private async void OnScoreboardClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new ScoreboardPage());
    }

    private async void OnNamesClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new PlayersPage());
    }

    private async void OnNewGameClicked(object? sender, EventArgs e)
    {
        await NavigationHelper.ConfirmAndStartNewGameAsync(this);
    }
}
