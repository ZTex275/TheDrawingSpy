namespace DrawingSpy.Pages;

public partial class SetupPage : ContentPage
{
    private const int MinPlayers = 3;
    private const int MaxPlayers = 15;
    private const int MinRounds = 1;
    private const int MaxRounds = 100;

    private readonly List<Entry> _nameEntries = new();
    private bool _syncingPlayers;
    private bool _syncingRounds;

    public SetupPage()
    {
        InitializeComponent();
        BuildNameEntries(ParsePlayersCount(fallback: 5));
    }

    private void OnPlayersStepperChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_syncingPlayers)
            return;

        int count = (int)e.NewValue;
        _syncingPlayers = true;
        PlayersCountEntry.Text = count.ToString();
        _syncingPlayers = false;
        BuildNameEntries(count);
    }

    private void OnRoundsStepperChanged(object? sender, ValueChangedEventArgs e)
    {
        if (_syncingRounds)
            return;

        _syncingRounds = true;
        RoundsCountEntry.Text = ((int)e.NewValue).ToString();
        _syncingRounds = false;
    }

    private void OnPlayersCountTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_syncingPlayers)
            return;

        if (int.TryParse(e.NewTextValue, out int count) && count >= MinPlayers && count <= MaxPlayers)
            BuildNameEntries(count);
    }

    private void OnPlayersCountUnfocused(object? sender, FocusEventArgs e)
    {
        ApplyPlayersCount(ParsePlayersCount(fallback: MinPlayers));
    }

    private void OnPlayersCountCompleted(object? sender, EventArgs e)
    {
        ApplyPlayersCount(ParsePlayersCount(fallback: MinPlayers));
    }

    private void OnRoundsCountUnfocused(object? sender, FocusEventArgs e)
    {
        ApplyRoundsCount(ParseRoundsCount(fallback: MinRounds));
    }

    private void OnRoundsCountCompleted(object? sender, EventArgs e)
    {
        ApplyRoundsCount(ParseRoundsCount(fallback: MinRounds));
    }

    private int ParsePlayersCount(int fallback)
    {
        if (int.TryParse(PlayersCountEntry.Text, out int value))
            return Math.Clamp(value, MinPlayers, MaxPlayers);
        return fallback;
    }

    private int ParseRoundsCount(int fallback)
    {
        if (int.TryParse(RoundsCountEntry.Text, out int value))
            return Math.Clamp(value, MinRounds, MaxRounds);
        return fallback;
    }

    private void ApplyPlayersCount(int count)
    {
        _syncingPlayers = true;
        PlayersCountEntry.Text = count.ToString();
        PlayersStepper.Value = count;
        _syncingPlayers = false;
        BuildNameEntries(count);
    }

    private void ApplyRoundsCount(int count)
    {
        _syncingRounds = true;
        RoundsCountEntry.Text = count.ToString();
        RoundsStepper.Value = count;
        _syncingRounds = false;
    }

    private void BuildNameEntries(int count)
    {
        var existing = _nameEntries.Select(en => en.Text).ToList();

        NamesContainer.Children.Clear();
        _nameEntries.Clear();

        for (int i = 0; i < count; i++)
        {
            var border = new Border
            {
                Stroke = Color.FromArgb("#3A2D6E"),
                StrokeThickness = 1,
                BackgroundColor = Color.FromArgb("#251A52"),
                Padding = new Thickness(12, 4),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 }
            };

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                ColumnSpacing = 10
            };

            var numberLabel = new Label
            {
                Text = $"{i + 1}.",
                TextColor = Color.FromArgb("#B9A4FF"),
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 26
            };

            var entry = new Entry
            {
                Placeholder = $"Игрок {i + 1}",
                Text = (i < existing.Count && !string.IsNullOrWhiteSpace(existing[i])) ? existing[i] : $"Игрок {i + 1}",
                TextColor = Colors.White,
                PlaceholderColor = Color.FromArgb("#8C80BE")
            };

            grid.Add(numberLabel, 0, 0);
            grid.Add(entry, 1, 0);
            border.Content = grid;

            NamesContainer.Children.Add(border);
            _nameEntries.Add(entry);
        }
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        int playerCount = ParsePlayersCount(fallback: MinPlayers);
        int rounds = ParseRoundsCount(fallback: MinRounds);

        ApplyPlayersCount(playerCount);
        ApplyRoundsCount(rounds);

        var game = App.Game;
        game.NewGame(playerCount, rounds);

        for (int i = 0; i < playerCount; i++)
            game.SetPlayerName(i, _nameEntries[i].Text);

        var roundIntro = new RoundIntroPage();
        Navigation.InsertPageBefore(roundIntro, this);
        await Navigation.PopAsync();
    }
}
