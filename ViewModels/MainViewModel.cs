using System.ComponentModel;

namespace AresLitho.ViewModels
{

    class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            PropertyChanged = delegate { }; // Initialize the event to avoid null issues
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
