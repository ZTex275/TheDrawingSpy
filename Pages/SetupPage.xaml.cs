namespace DrawingSpy.Pages;

public partial class SetupPage : ContentPage
{
    private readonly List<Entry> _nameEntries = new();

    public SetupPage()
    {
        InitializeComponent();
        BuildNameEntries((int)PlayersStepper.Value);
    }

    private void OnPlayersChanged(object? sender, ValueChangedEventArgs e)
    {
        int count = (int)e.NewValue;
        PlayersCountLabel.Text = count.ToString();
        BuildNameEntries(count);
    }

    private void OnRoundsChanged(object? sender, ValueChangedEventArgs e)
    {
        RoundsCountLabel.Text = ((int)e.NewValue).ToString();
    }

    private void BuildNameEntries(int count)
    {
        // Сохраняем уже введённые имена, чтобы не терять их при изменении количества.
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
        int playerCount = (int)PlayersStepper.Value;
        int rounds = (int)RoundsStepper.Value;

        var game = App.Game;
        game.NewGame(playerCount, rounds);

        for (int i = 0; i < playerCount; i++)
        {
            game.SetPlayerName(i, _nameEntries[i].Text);
        }

        var roundIntro = new RoundIntroPage();
        Navigation.InsertPageBefore(roundIntro, this);
        await Navigation.PopAsync();
    }
}
