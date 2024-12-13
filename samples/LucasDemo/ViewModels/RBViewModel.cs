using System;
using System.Windows.Input;
using MiniMvvm;

namespace LucasDemo.ViewModels
{
    public class RBViewModel : ViewModelBase
    {
        private string _text = $"Hello RBViewModel! {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}";
        public string Text
        {
            get => _text;
            set => RaiseAndSetIfChanged(ref _text, value);
        }

        public ICommand ChangeTextCommand { get; }
        
        private bool _NetworkChecked = true;

        public bool NetworkChecked
        {
            get => _NetworkChecked;
            set => RaiseAndSetIfChanged(ref _NetworkChecked, value);
        }


        public bool _SerialChecked = false;

        public bool SerialChecked
        {
            get => _SerialChecked;
            set => RaiseAndSetIfChanged(ref _SerialChecked, value);
        }

        public RBViewModel()
        {
            ChangeTextCommand = MiniCommand.Create(ChangeTextCommandHandler);
        }

        private void ChangeTextCommandHandler()
        {
            Text = $"Hello RBViewModel! {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}";
        }
    }
}
