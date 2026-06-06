using DrawingSpy.Pages;
using DrawingSpy.Services;

namespace DrawingSpy;

public partial class App : Application
{
    public static GameService Game { get; } = new();

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var nav = new NavigationPage(new SetupPage())
        {
            BarBackgroundColor = Color.FromArgb("#512BD4"),
            BarTextColor = Colors.White
        };
        return new Window(nav);
    }
}
