using AresLitho.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.Models.GooFile
{
    /// <summary>
    /// Gooファイルの各レイヤー情報
    /// </summary>
    public class GooLayerContent
    {
        public short PauseFlag { get; set; }
        public float PausePositionZ { get; set; }
        public float LayerPositionZ { get; set; }
        public float LayerExposureTime { get; set; }
        public float LayerOffTime { get; set; }

        public float BeforeLiftTime { get; set; }
        public float AfterLiftTime { get; set; }
        public float AfterRetractTime { get; set; }

        public float LiftDistance { get; set; }
        public float LiftSpeed { get; set; }

        public float SecondLiftDistance { get; set; }
        public float SecondLiftSpeed { get; set; }

        public float RetractDistance { get; set; }
        public float RetractSpeed { get; set; }

        public float SecondRetractDistance { get; set; }
        public float SecondRetractSpeed { get; set; }

        public short LightPwm { get; set; }

        public int DataSize { get; set; }
        public byte[]? ImageData { get; set; } // RLEエンコードされた画像データ

        public GooLayerContent()
        {
            SetDefaults();
        }

        public GooLayerContent(BinImage binImage)
        {
            SetDefaults();
            this.ImageData = GooUtils.EncodeImageDataFromBinaryWithRunLength(binImage.Byte);
            DataSize = this.ImageData.Length;
        }

        public GooLayerContent ApplyHeader(GooHeaderInfo header)
        {
            LayerExposureTime = header.CommonExposureTime;
            BeforeLiftTime = header.BeforeLiftTime;
            AfterLiftTime = header.AfterLiftTime;
            AfterRetractTime = header.AfterRetractTime;
            LiftDistance = header.LiftDistance;
            LiftSpeed = header.LiftSpeed;
            SecondLiftDistance = header.SecondLiftDistance;
            SecondLiftSpeed = header.SecondLiftSpeed;
            RetractDistance = header.RetractDistance;
            RetractSpeed = header.RetractSpeed;
            SecondRetractDistance = header.SecondRetractDistance;
            SecondRetractSpeed = header.SecondRetractSpeed;
            LightPwm = header.LightPwm;
            return this;
        }

        private void SetDefaults()
        {
            PauseFlag = 0;
            PausePositionZ = 0.001f;
            LayerPositionZ = 0.001f;
            LayerExposureTime = 300;
            LayerOffTime = 0;
            BeforeLiftTime = 0;
            AfterLiftTime = 0;
            AfterRetractTime = 0.001f;
            LiftDistance = 0.001f;
            LiftSpeed = 75;
            SecondLiftDistance = 0;
            SecondLiftSpeed = 230;
            RetractDistance = 0;
            RetractSpeed = 230;
            SecondRetractDistance = 0.001f;
            SecondRetractSpeed = 75;
            LightPwm = 255;
            DataSize = 0;
            ImageData = null;
        }

        public GooLayerContent(BigEndianBinaryReader br)
        {
            PauseFlag = br.ReadInt16();
            PausePositionZ = br.ReadSingle();
            LayerPositionZ = br.ReadSingle();
            LayerExposureTime = br.ReadSingle();
            LayerOffTime = br.ReadSingle();
            BeforeLiftTime = br.ReadSingle();
            AfterLiftTime = br.ReadSingle();
            AfterRetractTime = br.ReadSingle();
            LiftDistance = br.ReadSingle();
            LiftSpeed = br.ReadSingle();
            SecondLiftDistance = br.ReadSingle();
            SecondLiftSpeed = br.ReadSingle();
            RetractDistance = br.ReadSingle();
            RetractSpeed = br.ReadSingle();
            SecondRetractDistance = br.ReadSingle();
            SecondRetractSpeed = br.ReadSingle();
            LightPwm = br.ReadInt16();
            GooUtils.CheckCRLFDelimiter(br);
            DataSize = br.ReadInt32();
            ImageData = br.ReadBytes(DataSize);
            GooUtils.CheckCRLFDelimiter(br);
        }

        public void WriteToStream(Stream outputStream)
        {
            if (ImageData == null)
                throw new InvalidDataException("ImageData is null.");
            using BigEndianBinaryWriter bw = new(outputStream, Encoding.ASCII, true);
            bw.Write(PauseFlag);
            bw.Write(PausePositionZ);
            bw.Write(LayerPositionZ);
            bw.Write(LayerExposureTime);
            bw.Write(LayerOffTime);
            bw.Write(BeforeLiftTime);
            bw.Write(AfterLiftTime);
            bw.Write(AfterRetractTime);
            bw.Write(LiftDistance);
            bw.Write(LiftSpeed);
            bw.Write(SecondLiftDistance);
            bw.Write(SecondLiftSpeed);
            bw.Write(RetractDistance);
            bw.Write(RetractSpeed);
            bw.Write(SecondRetractDistance);
            bw.Write(SecondRetractSpeed);
            bw.Write(LightPwm);
            bw.Write([ 0x0d, 0x0a ]); // CRLF delimiter
            bw.Write(DataSize);
            bw.Write(ImageData);
            bw.Write([ 0x0d, 0x0a ]); // CRLF delimiter
        }

        public GooLayerContent Clone()
            => (GooLayerContent)MemberwiseClone();
    }
}
