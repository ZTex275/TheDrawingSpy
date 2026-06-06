namespace DrawingSpy.Controls;

public partial class NewGameConfirmOverlay : ContentView
{
    private TaskCompletionSource<bool>? _tcs;

    public NewGameConfirmOverlay()
    {
        InitializeComponent();
    }

    public Task<bool> ShowAsync()
    {
        _tcs = new TaskCompletionSource<bool>();
        IsVisible = true;
        return _tcs.Task;
    }

    private void Complete(bool confirmed)
    {
        IsVisible = false;
        _tcs?.TrySetResult(confirmed);
        _tcs = null;
    }

    private void OnCancelClicked(object? sender, EventArgs e) => Complete(false);

    private void OnConfirmClicked(object? sender, EventArgs e) => Complete(true);
}
