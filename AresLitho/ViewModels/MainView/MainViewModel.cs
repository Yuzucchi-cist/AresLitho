using System.ComponentModel;
using AresLitho.Commons;
using AresLitho.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AresLitho.Models.Layer;
using AresLitho.Models.PCBDxf;
using AresLitho.Commons.ExtendObservableCollection;
using AresLitho.Services;
using AresLitho.Models.GooFile;

namespace AresLitho.ViewModels.MainView
{
    class MainViewModel : ViewModelBase
    {
        public ICommand DragOverCommand { get; private set; }
        public ICommand DropCommand { get; private set; }

        public ICommand WriteGoo { get; private set; }

        public ExtendObservableCollection<Layer> Layers { get; } = [];

        private Layer? _SelectedLayer;
        public Layer? SelectedLayer
        {
            get { return _SelectedLayer; }
            set
            {
                if(_SelectedLayer == value) return;
                _SelectedLayer = value;
                if(value != null)
                {
                    Bitmap = EncodeToBitmap(value.BinImage);
                    _printerProfile = new PrinterProfileViewModel(value.PrinterProfile);
                }
                else
                {
                    Bitmap = null;
                    _printerProfile = null;
                }

                OnPropertyChanged(nameof(SelectedLayer));
                OnPropertyChanged(nameof(Bitmap));
                OnPropertyChanged(nameof(PrinterProfile));
            }
        }

        private BitmapSource? _Bitmap;
        public  BitmapSource? Bitmap { get { return _Bitmap; }
            private set
            {
                _Bitmap = value;
                OnPropertyChanged(nameof(Bitmap));
            }
        }

        private PrinterProfileViewModel? _printerProfile;
        public PrinterProfileViewModel? PrinterProfile
        {
            get => _printerProfile;
            set
            {
                _printerProfile = value;
                OnPropertyChanged(nameof(PrinterProfile));
            }
        }

        public MainViewModel() : base()
        {
            DragOverCommand = new RelayCommand<DragEventArgs>(DropArea_DragOver);
            DropCommand = new RelayCommand<DragEventArgs>(DropArea_DragDrop);
            WriteGoo = new RelayCommand<object>(WriteGoo_Execute);
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
            List<DxfFile> dxfFiles = [];

            foreach (var droppedFile in droppedFiles)
            {
                try
                {
                    dxfFiles.Add(DxfFile.Load(droppedFile));
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
            if (dxfFiles.Count == 0) return;
            Layers.AddRange(Layer.Load(dxfFiles));

            Bitmap = EncodeToBitmap(Layers[0].BinImage);

            // Drop event handling has done on DropArea
            e.Handled = true;
        }

        private void WriteGoo_Execute(object? parameter)
        {
            if (SelectedLayer is null) return;

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "GooFile",
                DefaultExt = ".goo",
                Filter = "Goo files (.goo)|*.goo"
            };

            bool? result = dialog.ShowDialog();
            if (result == false) return;

            try
            {
                string filename = dialog.FileName;

                BinImage binImage = SelectedLayer.BinImage;

                GooFile goo = GooFile.CreateFromBinImageToCenter(binImage);
                goo.WriteToFile(filename);
                MessageBox.Show($"{filename}は正常に書き込まれました。");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gooファイルの書き込み中にエラーが発生しました: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static BitmapSource EncodeToBitmap(BinImage image)
        {
            int width = (int)(20 / DxfRasterizer.pixelSizeMm), height = (int)(20 / DxfRasterizer.pixelSizeMm);
            byte[] importedImg = image.Resize(width, height).Invert(false, true).EncodeToBgr32();
            return BitmapSource.Create(width, height, 200, 200, PixelFormats.Bgr32, null, importedImg, width * 4);

        }
    }
}
