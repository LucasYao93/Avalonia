using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LucasDemo.Views;

public partial class RBView : UserControl
{
    public RBView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
