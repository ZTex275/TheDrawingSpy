using DrawingSpy.Controls;

namespace DrawingSpy.Helpers;

public static class ConfirmDialogHelper
{
    public static async Task<bool> ShowNewGameConfirmAsync(Page page)
    {
        if (page is not ContentPage contentPage || contentPage.Content is not Grid grid)
            return await FallbackAlertAsync(page);

        var overlay = new NewGameConfirmOverlay();
        grid.Children.Add(overlay);

        int rows = Math.Max(grid.RowDefinitions.Count, 1);
        int cols = Math.Max(grid.ColumnDefinitions.Count, 1);
        Grid.SetRow(overlay, 0);
        Grid.SetColumn(overlay, 0);
        Grid.SetRowSpan(overlay, rows);
        Grid.SetColumnSpan(overlay, cols);

        try
        {
            return await overlay.ShowAsync();
        }
        finally
        {
            grid.Children.Remove(overlay);
        }
    }

    private static Task<bool> FallbackAlertAsync(Page page) =>
        page.DisplayAlertAsync(
            "Новая игра",
            "Текущий прогресс будет сохранён. Настройки и имена игроков останутся на экране старта.",
            "Новая игра",
            "Отмена");
}
