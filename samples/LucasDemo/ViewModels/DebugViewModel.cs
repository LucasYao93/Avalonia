using MiniMvvm;

namespace LucasDemo.ViewModels
{
    public class DebugViewModel : ViewModelBase
    {
        public string Text { get;} = "Hello DebugViewModel!";

        public DebugViewModel()
        {
        }
    }
}
