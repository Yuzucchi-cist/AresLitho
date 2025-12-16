using AresLitho.Models.GooFile;

namespace AresLitho.Models
{
    internal class PrinterProfile
    {
        public float XSize { get; set; }
        public float YSize { get; set; }
        public ExposurePulse ExposurePulse;
        public PrinterProfile(float xSize, float ySize, float exposureTime)
        {
            XSize = xSize;
            YSize = ySize;
            ExposurePulse = new ExposurePulse(exposureTime, exposureTime, 0f);
        }

        public static PrinterProfile FromGoo(GooHeaderInfo gooHeaderInfo)
        {
            PrinterProfile profile = new(
                gooHeaderInfo.XSize,
                gooHeaderInfo.YSize,
                gooHeaderInfo.CommonExposureTime
            );
            return profile;
        }

        public GooFile.GooFile ApplyPrinterProfileToGoo(GooFile.GooFile goo)
        {
            goo.Header.XSize = XSize;
            goo.Header.YSize = YSize;
            goo.Header.CommonExposureTime = ExposurePulse.OnDuration;
            goo.Header.BottomExposureTime = ExposurePulse.OnDuration;
            goo.Header.AfterRetractTime = ExposurePulse.OffDuration;
            goo.Header.BottomAfterRetractTime = ExposurePulse.OffDuration;
            goo.Header.BottomLayers = 1;
            goo.Header.TransitionLayers = (short)(ExposurePulse.ExposureCount - goo.Header.BottomLayers);

            if (goo.Layers.Count < ExposurePulse.ExposureCount)
            {
                int layersToAdd = ExposurePulse.ExposureCount - goo.Layers.Count;
                for (int i = 0; i < layersToAdd; i++)
                {
                    goo.Layers.Add(CreateNextLayer(goo.Layers.Last(), goo.Header));
                }
            }
            else if (goo.Layers.Count > ExposurePulse.ExposureCount)
            {
                goo.Layers.RemoveRange(ExposurePulse.ExposureCount, goo.Layers.Count - ExposurePulse.ExposureCount);
            }

            goo.Layers.ForEach(layer =>
            {
                layer.LayerExposureTime = ExposurePulse.OnDuration;
                layer.AfterRetractTime = ExposurePulse.OffDuration;
            });


            return goo;
        }

        private GooLayerContent CreateNextLayer(GooLayerContent lastLayer, GooHeaderInfo header)
        {
            GooLayerContent newLayer = lastLayer.Clone();
            newLayer.ApplyHeader(header);
            newLayer.LayerPositionZ += header.LayerThickness;
            return newLayer;
        }
    }

    internal class ExposurePulse
    {
        public float ExposureTime { get; private set; }

        public float OnDuration { get; private set; }
        public float OffDuration { get; private set; }
        public float DutyRatio { get => OnDuration / (OnDuration + OffDuration); }
        public int ExposureCount { get => (int)(ExposureTime / OnDuration); }

        public ExposurePulse(float exposureTime, float onDuration, float offDuration)
        {
            ExposureTime = exposureTime;
            OnDuration = onDuration;
            OffDuration = offDuration;
        }

        public void SetExposureTime(float exposureTime)
        {
            if (ExposureTime < OnDuration)
                OnDuration = exposureTime;
            ExposureTime = exposureTime;
        }

        public void SetOnOff(float on, float off)
        {
            if (on < ExposureTime)
                OnDuration = on;
            else
                OnDuration = ExposureTime;
            OffDuration = off;
        }

        public void SetDutyRatio(float dutyRatio)
        {
            float total = OnDuration + OffDuration;
            OnDuration = total * DutyRatio;
            OffDuration = total * (1 - DutyRatio);
        }

        public void SetExposureCount(int count)
        {
            SetOnOff(ExposureTime / count, OffDuration);
        }
    }
}
