using AresLitho.Commons;
using System.Windows.Input;

namespace AresLitho.ViewModels.MainView
{
    public class PrinterProfileItemViewModel : ViewModelBase
    {
        public string Name { get; }
        public string Label { get; }
        public string Value
        {
            get => _getter().ToString() ?? "";
            set
            {
                _setter(value);
                OnPropertyChanged(nameof(Value));
            }
        }

        private Func<object> _getter;
        private Action<string> _setter;

        public ICommand ApplyValueOnEnterCommand { get; }

        public PrinterProfileItemViewModel(string name, string label, Func<object> getter, Action<string> setter) : base()
        {
            Name = name;
            Label = label;
            _getter = getter;
            _setter = setter;
            ApplyValueOnEnterCommand = new RelayCommand<KeyEventArgs>(ApplyValueOnEnter);
        }

        private void ApplyValueOnEnter(KeyEventArgs? e)
        {
            if (e == null) return;
            if (e.Key == Key.Enter)
            {
                _setter(Value);
                Keyboard.ClearFocus();
                OnPropertyChanged(nameof(Value));
            }
        }
    }

}
