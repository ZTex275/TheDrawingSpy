using DrawingSpy.Services;
using Microsoft.Maui.Controls.Shapes;

namespace DrawingSpy.Pages;

public partial class PlayersPage : ContentPage
{
    private readonly List<Entry> _entries = new();

    public PlayersPage()
    {
        InitializeComponent();
        BuildEntries();
    }

    private void BuildEntries()
    {
        var game = App.Game;
        NamesContainer.Children.Clear();
        _entries.Clear();

        for (int i = 0; i < game.Players.Count; i++)
        {
            var border = new Border
            {
                Stroke = Color.FromArgb("#3A2D6E"),
                StrokeThickness = 1,
                BackgroundColor = Color.FromArgb("#251A52"),
                Padding = new Thickness(12, 4),
                StrokeShape = new RoundRectangle { CornerRadius = 12 }
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

            grid.Add(new Label
            {
                Text = $"{i + 1}.",
                TextColor = Color.FromArgb("#B9A4FF"),
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 26
            }, 0, 0);

            var entry = new Entry
            {
                Text = game.Players[i].Name,
                Placeholder = $"Игрок {i + 1}",
                TextColor = Colors.White,
                PlaceholderColor = Color.FromArgb("#8C80BE")
            };
            grid.Add(entry, 1, 0);

            border.Content = grid;
            NamesContainer.Children.Add(border);
            _entries.Add(entry);
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var game = App.Game;
        for (int i = 0; i < _entries.Count; i++)
            game.SetPlayerName(i, _entries[i].Text);

        GameStorage.Save(game);
        await Navigation.PopAsync();
    }
}
