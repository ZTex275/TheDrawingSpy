namespace DrawingSpy.Pages;

public partial class RevealPage : ContentPage
{
    private const int TapDelayMs = 700;

    private int _currentTurn;
    private bool _revealed;
    private bool _busy;
    private CancellationTokenSource? _delayCts;

    public RevealPage()
    {
        InitializeComponent();
        ShowPassScreen();
    }

    protected override void OnDisappearing()
    {
        _delayCts?.Cancel();
        base.OnDisappearing();
    }

    private void ShowPassScreen()
    {
        var game = App.Game;
        var player = game.Players[_currentTurn];

        _revealed = false;
        StepLabel.Text = $"Игрок {_currentTurn + 1} из {game.Players.Count}";
        TitleLabel.Text = $"Передайте телефон:\n{player.Name}";
        WordBorder.IsVisible = false;
        HintLabel.Text = "Только этот игрок должен нажать кнопку ниже";
        RootGrid.BackgroundColor = Color.FromArgb("#1B1340");

        ActionButton.Text = "Показать слово";
        EnableActionButtonAfterDelay();
    }

    private void ShowRevealScreen()
    {
        var game = App.Game;
        var player = game.Players[_currentTurn];

        StepLabel.Text = player.Name;

        if (game.IsSpy(_currentTurn))
        {
            TitleLabel.Text = "Вы — ШПИОН!";
            WordLabel.Text = "🕵️";
            WordLabel.TextColor = Color.FromArgb("#FF6B6B");
            HintLabel.Text = "Слово вам неизвестно. Не выдайте себя!";
            RootGrid.BackgroundColor = Color.FromArgb("#3A0E1E");
        }
        else
        {
            TitleLabel.Text = "Ваше слово:";
            WordLabel.Text = game.CurrentWord;
            WordLabel.TextColor = Color.FromArgb("#B9A4FF");
            HintLabel.Text = "Запомните слово и скройте экран";
            RootGrid.BackgroundColor = Color.FromArgb("#152B1B");
        }

        WordBorder.IsVisible = true;
        ActionButton.Text = "Скрыть и передать дальше";
        EnableActionButtonAfterDelay();
    }

    private void EnableActionButtonAfterDelay()
    {
        _delayCts?.Cancel();
        _delayCts = new CancellationTokenSource();
        var token = _delayCts.Token;

        ActionButton.IsEnabled = false;
        ActionButton.Opacity = 0.45;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TapDelayMs, token);
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (token.IsCancellationRequested)
                        return;

                    ActionButton.IsEnabled = true;
                    ActionButton.Opacity = 1;
                });
            }
            catch (TaskCanceledException)
            {
            }
        });
    }

    private async void OnActionClicked(object? sender, EventArgs e)
    {
        if (_busy || !ActionButton.IsEnabled)
            return;

        var game = App.Game;

        if (!_revealed)
        {
            _revealed = true;
            ShowRevealScreen();
            return;
        }

        _currentTurn++;

        if (_currentTurn >= game.Players.Count)
        {
            _busy = true;
            ActionButton.IsEnabled = false;
            await Navigation.PushAsync(new OutcomePage());
            return;
        }

        ShowPassScreen();
    }
}
