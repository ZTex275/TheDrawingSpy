using DrawingSpy.Helpers;
using Microsoft.Maui.Controls.Shapes;

namespace DrawingSpy.Pages;

public partial class ScoreboardPage : ContentPage
{
    private readonly bool _roundSummary;

    public ScoreboardPage(bool roundSummary = false)
    {
        InitializeComponent();
        _roundSummary = roundSummary;

        if (_roundSummary)
            NavigationPage.SetHasBackButton(this, false);

        BuildTable();
        BuildSpyLog();
        BuildActions();
    }

    protected override bool OnBackButtonPressed()
    {
        // После завершения раунда назад ведёт на сломанный экран выбора исхода.
        if (_roundSummary)
            return true;

        return base.OnBackButtonPressed();
    }

    private void BuildTable()
    {
        var game = App.Game;
        int playedRounds = game.RoundScores.Count;

        TableGrid.Children.Clear();
        TableGrid.RowDefinitions.Clear();
        TableGrid.ColumnDefinitions.Clear();

        // Колонки: имя игрока + по колонке на сыгранный раунд + итог.
        TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        for (int r = 0; r < playedRounds; r++)
            TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(54) });
        TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(64) });

        // Строки: заголовок + по строке на игрока.
        TableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (int p = 0; p < game.Players.Count; p++)
            TableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Заголовок.
        AddCell("Игрок", 0, 0, header: true, alignStart: true);
        for (int r = 0; r < playedRounds; r++)
            AddCell($"Р{r + 1}", 0, r + 1, header: true);
        AddCell("Σ", 0, playedRounds + 1, header: true);

        // Найдём максимальный итог для подсветки лидера.
        int maxTotal = int.MinValue;
        for (int p = 0; p < game.Players.Count; p++)
            maxTotal = Math.Max(maxTotal, game.TotalScore(p));

        for (int p = 0; p < game.Players.Count; p++)
        {
            int total = game.TotalScore(p);
            bool leader = playedRounds > 0 && total == maxTotal && maxTotal > 0;

            AddCell(game.Players[p].Name, p + 1, 0, alignStart: true, leader: leader);

            for (int r = 0; r < playedRounds; r++)
            {
                int val = game.RoundScores[r][p];
                bool wasSpy = game.SpyPerRound[r] == p;
                AddCell(val.ToString(), p + 1, r + 1, spy: wasSpy);
            }

            AddCell(total.ToString(), p + 1, playedRounds + 1, total: true, leader: leader);
        }
    }

    private void AddCell(string text, int row, int col, bool header = false,
        bool total = false, bool alignStart = false, bool spy = false, bool leader = false)
    {
        Color bg;
        if (header) bg = Color.FromArgb("#7C4DFF");
        else if (total) bg = Color.FromArgb("#33285E");
        else if (spy) bg = Color.FromArgb("#5A1B2E");
        else bg = Color.FromArgb("#251A52");

        if (leader && (total || alignStart))
            bg = Color.FromArgb("#2E9E5B");

        var border = new Border
        {
            BackgroundColor = bg,
            Stroke = Color.FromArgb("#1B1340"),
            StrokeThickness = 1,
            Padding = new Thickness(8, 10),
            StrokeShape = new Rectangle()
        };

        var label = new Label
        {
            Text = text,
            TextColor = Colors.White,
            FontSize = header ? 14 : 15,
            FontAttributes = (header || total || alignStart) ? FontAttributes.Bold : FontAttributes.None,
            HorizontalTextAlignment = alignStart ? TextAlignment.Start : TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.TailTruncation
        };

        border.Content = label;
        TableGrid.Add(border, col, row);
    }

    private void BuildSpyLog()
    {
        var game = App.Game;
        SpyLog.Children.Clear();

        var title = new Label
        {
            Text = "Шпионы по раундам",
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            FontSize = 15
        };
        SpyLog.Children.Add(title);

        if (game.SpyPerRound.Count == 0)
        {
            SpyLog.Children.Add(new Label
            {
                Text = "Ещё не сыграно ни одного раунда.",
                TextColor = Color.FromArgb("#C7BFEA"),
                FontSize = 14
            });
            return;
        }

        for (int r = 0; r < game.SpyPerRound.Count; r++)
        {
            int spyIdx = game.SpyPerRound[r];
            SpyLog.Children.Add(new Label
            {
                Text = $"Раунд {r + 1}:  🕵️ {game.Players[spyIdx].Name}",
                TextColor = Color.FromArgb("#D8D2F0"),
                FontSize = 14
            });
        }
    }

    private void BuildActions()
    {
        var game = App.Game;
        ActionsContainer.Children.Clear();

        if (!_roundSummary)
        {
            // Режим просмотра (открыто из тулбара) — просто кнопка «Закрыть».
            var close = new Button
            {
                Text = "Закрыть",
                BackgroundColor = Color.FromArgb("#3A2D6E"),
                TextColor = Colors.White,
                FontSize = 16,
                HeightRequest = 50,
                CornerRadius = 12
            };
            close.Clicked += async (_, _) => await Navigation.PopAsync();
            ActionsContainer.Children.Add(close);
            return;
        }

        HeaderLabel.Text = $"Раунд {game.CurrentRound} завершён";

        if (game.CurrentRound < game.TotalRounds)
        {
            var next = new Button
            {
                Text = "Следующий раунд",
                BackgroundColor = Color.FromArgb("#7C4DFF"),
                TextColor = Colors.White,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 56,
                CornerRadius = 14
            };
            next.Clicked += async (_, _) => await NavigationHelper.GoToNextRoundAsync(Navigation);
            ActionsContainer.Children.Add(next);
        }
        else
        {
            // Игра окончена — определяем победителя(ей).
            int maxTotal = int.MinValue;
            for (int p = 0; p < game.Players.Count; p++)
                maxTotal = Math.Max(maxTotal, game.TotalScore(p));

            var winners = game.Players
                .Where(pl => game.TotalScore(pl.Index) == maxTotal)
                .Select(pl => pl.Name)
                .ToList();

            string winText = winners.Count == 1
                ? $"🏆 Победитель: {winners[0]} ({maxTotal})"
                : $"🏆 Ничья: {string.Join(", ", winners)} ({maxTotal})";

            ActionsContainer.Children.Add(new Label
            {
                Text = winText,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#9FE3BA"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            var newGame = new Button
            {
                Text = "Новая игра",
                BackgroundColor = Color.FromArgb("#7C4DFF"),
                TextColor = Colors.White,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 56,
                CornerRadius = 14
            };
            newGame.Clicked += (_, _) => NavigationHelper.GoToNewGame();
            ActionsContainer.Children.Add(newGame);
        }
    }
}
