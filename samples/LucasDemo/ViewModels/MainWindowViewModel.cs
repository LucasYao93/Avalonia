using System.Windows.Input;
using MiniMvvm;

namespace LucasDemo.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Title { get; } = "Main Window";

        private ViewModelBase _RBVM = new RBViewModel();

        private ViewModelBase _DebugVM = new DebugViewModel();

        private ViewModelBase _CurrentVM;

        public ICommand SwitchCurrentVMCommand { get; }

        public ViewModelBase CurrentVM
        {
            get => _CurrentVM;
            set => RaiseAndSetIfChanged(ref _CurrentVM, value);
        }

        public MainWindowViewModel()
        {
            CurrentVM = _RBVM;
            SwitchCurrentVMCommand = MiniCommand.Create(SwitchCurrentVM);
        }

        public void SwitchCurrentVM()
        {
            if (CurrentVM == _RBVM)
            {
                CurrentVM = _DebugVM;
            }
            else
            {
                CurrentVM = _RBVM;
            }
        }
    }
}
