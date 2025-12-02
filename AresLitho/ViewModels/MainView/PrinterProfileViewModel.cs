using AresLitho.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.ViewModels.MainView
{
    internal class PrinterProfileViewModel
    {
        private readonly PrinterProfile _model;
        public ObservableCollection<PrinterProfileItemViewModel> Items { get; }
        public PrinterProfileViewModel(PrinterProfile model)
        {
            _model = model;
            Items = new()
            {
                new(nameof(_model.XSize), "XSize",
                    () => _model.XSize.ToString(), v => _model.XSize = float.Parse(v)),
                new(nameof(_model.XSize), "YSize",
                    () => _model.YSize.ToString(), v => _model.YSize = float.Parse(v)),
                new(nameof(_model.ExposureTime), "露光時間",
                    () => _model.ExposureTime.ToString(), v => _model.ExposureTime = float.Parse(v)),
            };
        }
    }
}
