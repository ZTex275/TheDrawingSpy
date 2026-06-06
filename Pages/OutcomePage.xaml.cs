using DrawingSpy.Models;

namespace DrawingSpy.Pages;

public partial class OutcomePage : ContentPage
{
    private bool _handled;

    public OutcomePage()
    {
        InitializeComponent();
        HeaderLabel.Text = $"Раунд {App.Game.CurrentRound}: чем закончился?";
    }

    private Task ApplyAndContinue(RoundOutcome outcome)
    {
        if (_handled)
            return Task.CompletedTask;
        _handled = true;

        var game = App.Game;
        string spyName = game.Players[game.SpyIndex].Name;
        string word = game.CurrentWord;

        game.ApplyOutcome(outcome);

        SpyNameLabel.Text = spyName;
        WordRevealLabel.Text = word;
        RevealOverlay.IsVisible = true;
        return Task.CompletedTask;
    }

    private async void OnRevealContinue(object? sender, EventArgs e)
    {
        RevealOverlay.IsVisible = false;
        await Navigation.PushAsync(new ScoreboardPage(roundSummary: true));
    }

    private async void OnSpyFound(object? sender, EventArgs e) => await ApplyAndContinue(RoundOutcome.SpyFound);

    private async void OnSpyNotFound(object? sender, EventArgs e) => await ApplyAndContinue(RoundOutcome.SpyNotFound);

    private async void OnSpyGuessed(object? sender, EventArgs e) => await ApplyAndContinue(RoundOutcome.SpyGuessedWord);
}
