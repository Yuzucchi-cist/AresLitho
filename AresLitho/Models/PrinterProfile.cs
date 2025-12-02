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
        public float ExposureTime { get; set; }

        public PrinterProfile(float xSize, float ySize, float exposureTime)
        {
            XSize = xSize;
            YSize = ySize;
            ExposureTime = exposureTime;
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
    }
}
