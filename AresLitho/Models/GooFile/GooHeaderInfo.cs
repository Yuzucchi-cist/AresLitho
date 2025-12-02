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
    /// Gooファイルのヘッダー情報
    /// </summary>
    public class GooHeaderInfo
    {
        public readonly static int SmallPreviewImageWidth = 116;
        public readonly static int SmallPreviewImageHeight = 116;
        public readonly static int BigPreviewImageWidth = 290;
        public readonly static int BigPreviewImageHeight = 290;

        public  string Version { get; set; }         // 4 byte
        public  byte[] MagicTag { get; set; }        // 8 byte
        public  string SoftwareInfo { get; set; }    // 32 byte
        public  string SoftwareVersion { get; set; } // 24 byte
        public  string FileTime { get; set; }        // 24 byte
        public  string PrinterName { get; set; }     // 32 byte
        public  string PrinterType { get; set; }     // 32 byte
        public  string ProfileName { get; set; }     // 32 byte

        public  short AntiAliasingLevel { get; set; }    // 2 byte
        public  short GreyLevel { get; set; }            // 2 byte
        public  short BlurLevel { get; set; }            // 2 byte

        public byte[]? SmallPreviewImage { get; set; }  // 2*116*116 byte RGB565, 116x116
        public byte[]? BigPreviewImage { get; set; }    // 2*290*290 byte RGB565, 290x290

        private int? _totalLayers;
        public  int? TotalLayers
        {
            get
            {
                if (_totalLayers is null)
                    return BottomLayers + TransitionLayers;
                return _totalLayers;
            }
            private set
            {
                _totalLayers = value;
            }
        }   // 4 byte

        public  short XResolution { get; set; }  // 2 byte
        public  short YResolution { get; set; }  // 2 byte

        public  bool XMirror { get; set; }   // 1 byte
        public  bool YMirror { get; set; }   // 1 byte

        public  float XSize { get; set; }            // 4 byte
        public  float YSize { get; set; }            // 4 byte
        public  float ZSize { get; set; }            // 4 byte
        public  float LayerThickness { get; set; }   // 4 byte

        public  float CommonExposureTime { get; set; }   // 1 byte
        public  bool ExposureDelayMode { get; set; }     // 4 byte

        public  float TurnOffTime { get; set; }          // 4 byte

        public  float BottomBeforeLiftTime { get; set; }     // 4 byte
        public  float BottomAfterLiftTime { get; set; }      // 4 byte
        public  float BottomAfterRetractTime { get; set; }   // 4 byte

        public  float BeforeLiftTime { get; set; }   // 4 byte
        public  float AfterLiftTime { get; set; }    // 4 byte
        public  float AfterRetractTime { get; set; } // 4 byte

        public  float BottomExposureTime { get; set; }   // 4 byte
        public  int BottomLayers { get; set; }           // 4 byte

        public  float BottomLiftDistance { get; set; }   // 4 byte
        public  float BottomLiftSpeed { get; set; }      // 4 byte

        public  float LiftDistance { get; set; }         // 4 byte
        public  float LiftSpeed { get; set; }            // 4 byte

        public  float BottomRetractDistance { get; set; }    // 4 byte
        public  float BottomRetractSpeed { get; set; }       // 4 byte

        public  float RetractDistance { get; set; }  // 4 byte
        public  float RetractSpeed { get; set; }     // 4 byte

        public  float BottomSecondLiftDistance { get; set; } // 4 byte
        public  float BottomSecondLiftSpeed { get; set; }    // 4 byte

        public  float SecondLiftDistance { get; set; }       // 4 byte
        public  float SecondLiftSpeed { get; set; }          // 4 byte

        public  float BottomSecondRetractDistance { get; set; }  // 4 byte
        public  float BottomSecondRetractSpeed { get; set; }     // 4 byte

        public  float SecondRetractDistance { get; set; }    // 4 byte
        public  float SecondRetractSpeed { get; set; }       // 4 byte

        public  short BottomLightPwm { get; set; }   // 2 byte
        public  short LightPwm { get; set; }         // 2 byte

        public  bool AdvanceMode { get; set; }   // 1 byte

        public  int PrintingTime { get; set; }   // 4 byte
        public  float TotalVolume { get; set; }  // 4 byte
        public  float TotalWeight { get; set; }  // 4 byte
        public  float TotalPrice { get; set; }   // 4 byte
        public  string PriceUnit { get; set; }   // 4 byte

        public  int? OffsetOfLayerContent { get; set; }   // 4 byte
        public  bool GrayScaleLevel { get; set; }        // 1 byte
        public  short TransitionLayers { get; set; }     // 2 byte
        public GooHeaderInfo()
        {
            Version = string.Empty;
            MagicTag = [0x07, 0x00, 0x00, 0x00, 0x44, 0x4C, 0x50, 0x00];
            SoftwareInfo = string.Empty;
            SoftwareVersion = string.Empty;
            FileTime = string.Empty;
            PrinterName = string.Empty;
            PrinterType = string.Empty;
            ProfileName = string.Empty;
            PriceUnit = string.Empty;
        }

        public static GooHeaderInfo CreateDefaultGooHeaderInfo()
        {
            return new GooHeaderInfo
            {
                Version = "V3.0",
                MagicTag = [0x07, 0x00, 0x00, 0x00, 0x44, 0x4C, 0x50, 0x00],
                SoftwareInfo = "AresLitho",
                SoftwareVersion = "0.0.10",
                FileTime = "0",
                PrinterName = "ELEGOO Mars 4",
                PrinterType = "ELEGOO Mars 4",
                ProfileName = "MARS 4*1",
                AntiAliasingLevel = 2,
                GreyLevel = 0,
                BlurLevel = 2,
                SmallPreviewImage = new byte[2 * 116 * 116], // 空のプレビュー画像
                BigPreviewImage = new byte[2 * 290 * 290],   // 空のプレビュー画像
                XResolution = 8520,
                YResolution = 4320,
                XMirror = true,
                YMirror = false,
                XSize = 153.36f,
                YSize = 77.76f,
                ZSize = 0.01f,
                LayerThickness = 0.01f,
                CommonExposureTime = 300,
                ExposureDelayMode = false,
                TurnOffTime = 0,
                BottomBeforeLiftTime = 0,
                BottomAfterLiftTime = 0,
                BottomAfterRetractTime = 0,
                BeforeLiftTime = 0,
                AfterLiftTime = 0,
                AfterRetractTime = 0,
                BottomExposureTime = 0,
                BottomLayers = 0,
                BottomLiftDistance = 0,
                BottomLiftSpeed = 100,
                LiftDistance = 0,
                LiftSpeed = 100,
                BottomRetractDistance = 0,
                BottomRetractSpeed = 230,
                RetractDistance = 0,
                RetractSpeed = 230,
                BottomSecondLiftDistance = 0,
                BottomSecondLiftSpeed = 230,
                SecondLiftDistance = 0,
                SecondLiftSpeed = 230,
                BottomSecondRetractDistance = 0,
                BottomSecondRetractSpeed = 75,
                SecondRetractDistance = 0,
                SecondRetractSpeed = 75,
                BottomLightPwm = 255,
                LightPwm = 255,
                AdvanceMode = false,
                PrintingTime = 1,
                TotalVolume = 1,
                TotalWeight = 1,
                TotalPrice = 1,
                PriceUnit = "$/L",
                OffsetOfLayerContent = null,
                GrayScaleLevel = true,
                TransitionLayers = 1
            };
        }

        public void WriteToStream(Stream outputStream)
        {
            if (SmallPreviewImage == null || SmallPreviewImage.Length != 2 * 116 * 116)
                throw new InvalidDataException("SmallPreviewImage is null.");
            if (BigPreviewImage == null || BigPreviewImage.Length != 2 * 290 * 290)
                throw new InvalidDataException("BigPreviewImage is null.");
            if (TotalLayers == null)
                throw new InvalidDataException("TotalLayers is null.");

            using BigEndianBinaryWriter bw = new(outputStream, Encoding.ASCII, true);
            Encoding enc = Encoding.ASCII;

            void WriteFixedString(string s, int length)
            {
                byte[] bytes = new byte[length];
                byte[] strBytes = enc.GetBytes(s);
                int copyLength = Math.Min(strBytes.Length, length);
                Array.Copy(strBytes, bytes, copyLength);
                bw.Write(bytes);
            }

            WriteFixedString(Version, 4);
            bw.Write(MagicTag);
            WriteFixedString(SoftwareInfo, 32);
            WriteFixedString(SoftwareVersion, 24);
            WriteFixedString(FileTime, 24);
            WriteFixedString(PrinterName, 32);
            WriteFixedString(PrinterType, 32);
            WriteFixedString(ProfileName, 32);
            bw.Write(AntiAliasingLevel);
            bw.Write(GreyLevel);
            bw.Write(BlurLevel);
            bw.Write(SmallPreviewImage);
            bw.Write([0x0d, 0x0a]); // CRLF delimiter
            bw.Write(BigPreviewImage);
            bw.Write([0x0d, 0x0a]); // CRLF delimiter
            bw.Write((int)TotalLayers);
            bw.Write(XResolution);
            bw.Write(YResolution);
            bw.Write(XMirror);
            bw.Write(YMirror);
            bw.Write(XSize);
            bw.Write(YSize);
            bw.Write(ZSize);
            bw.Write(LayerThickness);
            bw.Write(CommonExposureTime);
            bw.Write(ExposureDelayMode);
            bw.Write(TurnOffTime);
            bw.Write(BottomBeforeLiftTime);
            bw.Write(BottomAfterLiftTime);
            bw.Write(BottomAfterRetractTime);
            bw.Write(BeforeLiftTime);
            bw.Write(AfterLiftTime);
            bw.Write(AfterRetractTime);
            bw.Write(BottomExposureTime);
            bw.Write(BottomLayers);
            bw.Write(BottomLiftDistance);
            bw.Write(BottomLiftSpeed);
            bw.Write(LiftDistance);
            bw.Write(LiftSpeed);
            bw.Write(BottomRetractDistance);
            bw.Write(BottomRetractSpeed);
            bw.Write(RetractDistance);
            bw.Write(RetractSpeed);
            bw.Write(BottomSecondLiftDistance);
            bw.Write(BottomSecondLiftSpeed);
            bw.Write(SecondLiftDistance);
            bw.Write(SecondLiftSpeed);
            bw.Write(BottomSecondRetractDistance);
            bw.Write(BottomSecondRetractSpeed);
            bw.Write(SecondRetractDistance);
            bw.Write(SecondRetractSpeed);
            bw.Write(BottomLightPwm);
            bw.Write(LightPwm);
            bw.Write(AdvanceMode);
            bw.Write(PrintingTime);
            bw.Write(TotalVolume);
            bw.Write(TotalWeight);
            bw.Write(TotalPrice);
            WriteFixedString(PriceUnit, 8);
            // レイヤーコンテンツのオフセットを一時的に0で書き込む
            long PositionOfOffsetOfLayerContent = bw.BaseStream.Position;
            bw.Write(0);
            bw.Write(GrayScaleLevel);
            bw.Write(TransitionLayers);

            // レイヤーコンテンツの正しいオフセットを書き込む
            long layerOffset = bw.BaseStream.Position;
            bw.BaseStream.Seek(PositionOfOffsetOfLayerContent, SeekOrigin.Begin);
            bw.Write((int)layerOffset);
            bw.BaseStream.Seek(layerOffset, SeekOrigin.Begin);
        }

        public GooHeaderInfo(BigEndianBinaryReader br)
        {
            Encoding enc = Encoding.ASCII;

            Version = ReadFixedString(br, 4, enc);
            MagicTag = br.ReadBytes(8);
            SoftwareInfo = ReadFixedString(br, 32, enc);
            SoftwareVersion = ReadFixedString(br, 24, enc);
            FileTime = ReadFixedString(br, 24, enc);
            PrinterName = ReadFixedString(br, 32, enc);
            PrinterType = ReadFixedString(br, 32, enc);
            ProfileName = ReadFixedString(br, 32, enc);

            AntiAliasingLevel = br.ReadInt16();
            GreyLevel = br.ReadInt16();
            BlurLevel = br.ReadInt16();

            SmallPreviewImage = br.ReadBytes(2 * 116 * 116);
            GooUtils.CheckCRLFDelimiter(br);

            BigPreviewImage = br.ReadBytes(2 * 290 * 290);
            GooUtils.CheckCRLFDelimiter(br);

            TotalLayers = br.ReadInt32();
            XResolution = br.ReadInt16();
            YResolution = br.ReadInt16();
            XMirror = br.ReadBoolean();
            YMirror = br.ReadBoolean();
            XSize = br.ReadSingle();
            YSize = br.ReadSingle();
            ZSize = br.ReadSingle();
            LayerThickness = br.ReadSingle();
            CommonExposureTime = br.ReadSingle();
            ExposureDelayMode = br.ReadBoolean();
            TurnOffTime = br.ReadSingle();
            BottomBeforeLiftTime = br.ReadSingle();
            BottomAfterLiftTime = br.ReadSingle();
            BottomAfterRetractTime = br.ReadSingle();
            BeforeLiftTime = br.ReadSingle();
            AfterLiftTime = br.ReadSingle();
            AfterRetractTime = br.ReadSingle();
            BottomExposureTime = br.ReadSingle();
            BottomLayers = br.ReadInt32();
            BottomLiftDistance = br.ReadSingle();
            BottomLiftSpeed = br.ReadSingle();
            LiftDistance = br.ReadSingle();
            LiftSpeed = br.ReadSingle();
            BottomRetractDistance = br.ReadSingle();
            BottomRetractSpeed = br.ReadSingle();
            RetractDistance = br.ReadSingle();
            RetractSpeed = br.ReadSingle();
            BottomSecondLiftDistance = br.ReadSingle();
            BottomSecondLiftSpeed = br.ReadSingle();
            SecondLiftDistance = br.ReadSingle();
            SecondLiftSpeed = br.ReadSingle();
            BottomSecondRetractDistance = br.ReadSingle();
            BottomSecondRetractSpeed = br.ReadSingle();
            SecondRetractDistance = br.ReadSingle();
            SecondRetractSpeed = br.ReadSingle();
            BottomLightPwm = br.ReadInt16();
            LightPwm = br.ReadInt16();
            AdvanceMode = br.ReadBoolean();
            PrintingTime = br.ReadInt32();
            TotalVolume = br.ReadSingle();
            TotalWeight = br.ReadSingle();
            TotalPrice = br.ReadSingle();
            PriceUnit = ReadFixedString(br, 8, enc);
            OffsetOfLayerContent = br.ReadInt32();
            GrayScaleLevel = br.ReadBoolean();
            TransitionLayers = br.ReadInt16();
        }

        private static string ReadFixedString(BigEndianBinaryReader br, int length, Encoding enc)
        {
            byte[] bytes = br.ReadBytes(length);
            string s = enc.GetString(bytes);
            int nullIndex = s.IndexOf('\0');
            if (nullIndex >= 0) s = s[..nullIndex];
            return s.TrimEnd();
        }
    }

}
