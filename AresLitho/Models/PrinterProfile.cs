using AresLitho.Models.GooFile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                gooHeaderInfo.BottomExposureTime
            );
            return profile;
        }

        public GooFile.GooFile ApplyPrinterProfileToGoo(GooFile.GooFile goo)
        {
            goo.Header.XSize = XSize;
            goo.Header.YSize = YSize;
            goo.Header.BottomExposureTime = ExposurePulse.ExposureTime;
            return goo;
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
