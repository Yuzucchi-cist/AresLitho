using System.ComponentModel;
using AresLitho.Commons;
using AresLitho.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AresLitho.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public ICommand DragOverCommand { get; private set; }
        public ICommand DropCommand { get; private set; }

        public ObservableCollection<ImportedFile> ImportedFiles { get; } = new();

        private BitmapSource? _Bitmap;
        public  BitmapSource? Bitmap { get { return _Bitmap; }
            private set
            {
                _Bitmap = value;
                OnPropertyChanged(nameof(Bitmap));
            }
        }

        public MainViewModel()
        {
            DragOverCommand = new RelayCommand<DragEventArgs>(DropArea_DragOver);
            DropCommand = new RelayCommand<DragEventArgs>(DropArea_DragDrop);
            PropertyChanged = delegate { }; // Initialize the event to avoid null issues
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void DropArea_DragOver(DragEventArgs? e)
        {
            if (e == null) return;
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                      ? DragDropEffects.Copy
                      : DragDropEffects.None;

            // Drag over event handling has done on DropArea
            e.Handled = true;
        }

        private void DropArea_DragDrop(DragEventArgs? e)
        {
            if (e?.Data.GetData(DataFormats.FileDrop) is not string[] droppedFiles) return;

            Exception[] unimportedFileExceptions = [];
            foreach (var droppedFile in droppedFiles)
            {
                try
                {
                    ImportedFiles.Add(new ImportedFile(droppedFile));
                }
                catch (ArgumentException exception)
                {
                    unimportedFileExceptions = [.. unimportedFileExceptions, exception];
                }
            }

            if (unimportedFileExceptions.Length != 0)
            {
                // Show Error MessageBox if unimported files exist.
                MessageBox.Show(
                    "以下のファイルはインポートされませんでした。:\n" +
                    string.Join("\n", unimportedFileExceptions.Select(ex => $"- {ex.Message}")),
                    "Import Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }

            byte[] importedImg = ImportedFiles[0].Bgr32Image!;
            int width = ImportedFiles[0].BinImage!.Width, height = ImportedFiles[0].BinImage!.Height;
            Bitmap = BitmapSource.Create(width, height, 200, 200, PixelFormats.Bgr32, null, importedImg, width * 4);

            // Drop event handling has done on DropArea
            e.Handled = true;
        }
    }
}
