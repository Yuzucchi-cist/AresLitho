using AresLitho.Models;
using System.Collections.ObjectModel;

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
                new(nameof(_model.ExposurePulse.ExposureTime), "露光時間",
                    () => _model.ExposurePulse.ExposureTime.ToString(), SetExposureTime),
                new(nameof(_model.ExposurePulse.OnDuration), "連続ON時間",
                    () => _model.ExposurePulse.OnDuration.ToString(), SetOnDuration),
                new(nameof(_model.ExposurePulse.OffDuration), "連続OFF時間",
                    () => _model.ExposurePulse.OffDuration.ToString(), SetOffDuration),
                new(nameof(_model.ExposurePulse.DutyRatio), "デューティ比",
                    () => _model.ExposurePulse.DutyRatio.ToString(), SetDutyRatio),
                new(nameof(_model.ExposurePulse.ExposureCount), "露光回数",
                    () => _model.ExposurePulse.ExposureCount.ToString(), SetExposureCount),
            };
        }

        private void SetExposureTime(string v)
        {
            if (!float.TryParse(v, out float result)) return;
            _model.ExposurePulse.SetExposureTime(result);
            NotifyAllPulseItems();
        }
        private void SetOnDuration(string v)
        {
            if (!float.TryParse(v, out float result)) return;
            _model.ExposurePulse.SetOnOff(result, _model.ExposurePulse.OffDuration);
            NotifyAllPulseItems();
        }
        private void SetOffDuration(string v)
        {
            if (!float.TryParse(v, out float result)) return;
            _model.ExposurePulse.SetOnOff(_model.ExposurePulse.OnDuration, result);
            NotifyAllPulseItems();
        }
        private void SetDutyRatio(string v)
        {
            if (!float.TryParse(v, out float result)) return;
            _model.ExposurePulse.SetDutyRatio(result);
            NotifyAllPulseItems();
        }
        private void SetExposureCount(string v)
        {
            if (!int.TryParse(v, out int result)) return;
            _model.ExposurePulse.SetExposureCount(result);
            NotifyAllPulseItems();
        }

        private void NotifyAllPulseItems()
        {
            foreach (var item in Items)
                if (item.Name
                    is nameof(_model.ExposurePulse.ExposureTime)
                    or nameof(_model.ExposurePulse.OnDuration)
                    or nameof(_model.ExposurePulse.OffDuration)
                    or nameof(_model.ExposurePulse.DutyRatio)
                    or nameof(_model.ExposurePulse.ExposureCount))
                    item.OnPropertyChanged(nameof(item.Value));
        }
    }
}
