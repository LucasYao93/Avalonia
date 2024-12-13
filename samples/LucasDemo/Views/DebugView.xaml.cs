using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LucasDemo.Views;

public partial class DebugView : UserControl
{
    public DebugView()
    {
        InitializeComponent();

        DataContext = new LucasDemo.ViewModels.DebugViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
