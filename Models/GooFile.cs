using System.IO;
using System.Text;
using AresLitho.Commons;

namespace AresLitho.Models
{
    /// <summary>
    /// Gooファイル全体を表すクラス
    /// </summary>
    public class GooFile
    {
        public GooHeaderInfo Header { get; set; }
        public List<GooLayerContent> Layers { get; set; } = [];

        public GooFile(GooHeaderInfo header, List<GooLayerContent> layers)
        {
            Header = header;
            Layers = layers;
        }

        public GooFile()
        {
            Layers = [];
            Header = GooHeaderInfo.CreateDefaultGooHeaderInfo();
            Header.TotalLayers = Layers.Count;
        }
        public static GooFile CreateFromBinImage2cmToCenter(BinImage layerImage)
        {
            GooFile goo = new()
            {
                Layers = [],
                Header = GooHeaderInfo.CreateDefaultGooHeaderInfo()
            };

            goo.Header.SmallPreviewImage = layerImage.Resize(GooHeaderInfo.SmallPreviewImageWidth, GooHeaderInfo.SmallPreviewImageHeight).EncodeToBinary();
            goo.Header.BigPreviewImage = layerImage.Resize(GooHeaderInfo.BigPreviewImageWidth, GooHeaderInfo.BigPreviewImageHeight).EncodeToBinary();
            goo.Header.TotalLayers = 1;

            int xSize2cm = (int)(2 * goo.Header.XResolution / goo.Header.XSize);
            int ySize2cm = (int)(2 * goo.Header.YResolution / goo.Header.YSize);

            layerImage = new BinImage(goo.Header.XResolution, goo.Header.YResolution).SetToCenter(layerImage.Resize(xSize2cm, ySize2cm));

            goo.Layers.Add(new GooLayerContent(layerImage));

            return goo;
        }

        public static GooFile CreateFromBinImage(BinImage layerImage)
        {
            GooFile goo = new()
            {
                Layers = [],
                Header = GooHeaderInfo.CreateDefaultGooHeaderInfo()
            };

            goo.Header.SmallPreviewImage = layerImage.Resize(GooHeaderInfo.SmallPreviewImageWidth, GooHeaderInfo.SmallPreviewImageHeight).EncodeToBinary();
            goo.Header.BigPreviewImage = layerImage.Resize(GooHeaderInfo.BigPreviewImageWidth, GooHeaderInfo.BigPreviewImageHeight).EncodeToBinary();
            goo.Header.TotalLayers = 1;

            goo.Layers.Add(new GooLayerContent(layerImage.Resize(goo.Header.XResolution, goo.Header.YResolution)));

            return goo;
        }

        public GooFile(string filePath)
        {
            using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            using BigEndianBinaryReader br = new(fs);
            try
            {
                Header = new GooHeaderInfo(br);
                Layers = [];
                for (int i = 0; i < Header.TotalLayers; i++)
                {
                    GooLayerContent layer = new(br);
                    Layers.Add(layer);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Failed to read Goo file.", ex);
            }
        }
        public void WriteToFile(string filePath)
        {
            using FileStream fs = new(filePath, FileMode.Create, FileAccess.Write);
            WriteToStream(fs);
        }
        public void WriteToStream(Stream outputStream)
        {
            Header.TotalLayers = Layers.Count;
            Header.WriteToStream(outputStream);
            foreach (var layer in Layers)
            {
                layer.WriteToStream(outputStream);
            }
            
            using BigEndianBinaryWriter bw = new(outputStream, Encoding.ASCII, true);
            bw.Write([0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x44, 0x4C, 0x50, 0x00]);   // Write Ending string
        }
    }

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

        public  int? TotalLayers { get; set; }   // 4 byte

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
                TotalLayers = 1,
                XResolution = 8520,
                YResolution = 4320,
                XMirror = true,
                YMirror = false,
                XSize = 153.36f,
                YSize = 77.76f,
                ZSize = 0.01f,
                LayerThickness = 0.01f,
                CommonExposureTime = 300,
                ExposureDelayMode = true,
                TurnOffTime = 0,
                BottomBeforeLiftTime = 0.2f,
                BottomAfterLiftTime = 0,
                BottomAfterRetractTime = 0.5f,
                BeforeLiftTime = 0.2f,
                AfterLiftTime = 0,
                AfterRetractTime = 0.5f,
                BottomExposureTime = 300,
                BottomLayers = 1,
                BottomLiftDistance = 2,
                BottomLiftSpeed = 100,
                LiftDistance = 10,
                LiftSpeed = 100,
                BottomRetractDistance = 3,
                BottomRetractSpeed = 230,
                RetractDistance = 3,
                RetractSpeed = 230,
                BottomSecondLiftDistance = 3,
                BottomSecondLiftSpeed = 230,
                SecondLiftDistance = 3,
                SecondLiftSpeed = 230,
                BottomSecondRetractDistance = 2,
                BottomSecondRetractSpeed = 75,
                SecondRetractDistance = 2,
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
                TransitionLayers = 0
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

        private void SetDefaults()
        {
            PauseFlag = 0;
            PausePositionZ = 0.01f;
            LayerPositionZ = 0.01f;
            LayerExposureTime = 300;
            LayerOffTime = 0;
            BeforeLiftTime = 0.2f;
            AfterLiftTime = 0;
            AfterRetractTime = 0.5f;
            LiftDistance = 2;
            LiftSpeed = 75;
            SecondLiftDistance = 3;
            SecondLiftSpeed = 230;
            RetractDistance = 3;
            RetractSpeed = 230;
            SecondRetractDistance = 2;
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
    }

    internal static class GooUtils
    {
        /// <summary>
        /// Checks if the next two bytes read from a BinaryReader are the CRLF delimiter (0x0d, 0x0a).
        /// </summary>
        public static void CheckCRLFDelimiter(BinaryReader br)
        {
            byte[] delimiter = br.ReadBytes(2);
            if (delimiter[0] == 0x0d && delimiter[1] == 0x0a)
            {
                return;
            }
            else
            {
                throw new InvalidDataException("CRLF delimiter not found where expected.");
            }
        }

        public static bool[,] ResizeBinImage(bool[,] image, int newWidth, int newHeight)
        {
            bool[,] newImage = new bool[newHeight, newWidth];
            double scaleY = (double)newHeight / image.GetLength(0);
            double scaleX = (double)newWidth / image.GetLength(1);
            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    newImage[y, x] = image[(int)(y / scaleY), (int)(x / scaleX)];
                }
            }
            return newImage;
        }

        public static byte[] EncodeImageDataFromBinary(bool[,] data)
        {
            int height = data.GetLength(0);
            int width = data.GetLength(1);
            byte[] imageData = new byte[height * width * 2];
            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ushort pixelValue = (ushort)(data[y, x] ? 0xFFFF : 0x0000);
                    imageData[index++] = (byte)((pixelValue >> 8) & 0xFF); // High byte
                    imageData[index++] = (byte)(pixelValue & 0xFF);        // Low byte
                }
            }
            return imageData;
        }

        /// <summary>
        /// Encodes a 2D boolean array into a compressed byte array using run-length encoding.
        /// byte[,] データから RLE圧縮された ImageData を生成する。
        /// </summary>
        /// <param name="data">A 2D array representing byte image data to be encoded.</param>
        /// <returns>A byte array containing the encoded image data along with a checksum.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided 2D array is null.</exception>
        public static byte[] EncodeImageDataFromBinaryWithRunLength(byte[,] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            int height = data.GetLength(0);
            int width = data.GetLength(1);

            List<byte> bytes = [];

            // Magic number (仕様より)
            bytes.Add(0x55);

            // 1行ずつ走査（左→右、上→下）
            for (int y = 0; y < height; y++)
            {
                byte current = data[y, 0];
                int run = 1;

                for (int x = 1; x < width; x++)
                {
                    if (data[y, x] == current)
                    {
                        run++;
                        // RLEランが最大長を超えないように分割
                        if (run >= 0xFFF) // 任意制限
                        {
                            AppendRleChunk(bytes, current, run);
                            run = 0;
                        }
                    }
                    else
                    {
                        AppendRleChunk(bytes, current, run);
                        current = data[y, x];
                        run = 1;
                    }
                }

                AppendRleChunk(bytes, current, run);
            }

            // チェックサム（仕様：0x55を除く全バイトの8bit和）
            int checksum = 0;
            for (int i = 1; i < bytes.Count; i++)
                checksum += bytes[i];
            checksum = ~checksum & 0xff;
            bytes.Add((byte)checksum);

            return [.. bytes];
        }

        /// <summary>
        /// Goo仕様の簡略RLEチャンクを追加する（全0 or 全FFのみ対応）
        /// </summary>
        private static void AppendRleChunk(List<byte> bytes, byte value, int runLength)
        {
            byte typeBits;
            // ---- 1. チャンクタイプ決定 ----
            if (value == 0x00)
                typeBits = 0b0000_0000;
            else if (value == 0xFF)
                typeBits = 0b1100_0000;
            else
                typeBits = 0b0100_0000;

            if (runLength < 16)
            {
                // 4bitラン長
                byte b0 = (byte)(typeBits | (runLength & 0x0F));
                bytes.Add(b0);
            }
            else if (runLength < 0b0001_0000_0000_0000)
            {
                // 2バイトラン長 (byte0[5:4]=01)
                byte b0 = (byte)(typeBits | (runLength & 0x0F));
                byte b1 = (byte)((runLength >> 4) & 0x0F);
                bytes.Add(b0);
                bytes.Add(b1);
            }
            else if (runLength < 0b0001_0000_0000_0000_0000_0000)
            {
                // 3バイトラン長 (byte0[5:4]=10)
                byte b0 = (byte)(typeBits | (runLength & 0x0F));
                byte b1 = (byte)((runLength >> 4) & 0x0F);
                byte b2 = (byte)((runLength >> 12) & 0x0F);
                bytes.Add(b0);
                bytes.Add(b1);
                bytes.Add(b2);
            }
            else
            {
                // 4バイトラン長 (byte0[5:4]=11)
                byte b0 = (byte)(typeBits | 0b00010000 | (runLength & 0x0F));
                byte b1 = (byte)((runLength >> 4) & 0xFF);
                byte b2 = (byte)((runLength >> 12) & 0x0F);
                byte b3 = (byte)((runLength >> 20) & 0x0F);
                bytes.Add(b0);
                bytes.Add(b1);
                bytes.Add(b2);
                bytes.Add(b3);
            }

            if (value != 0x00 && value != 0xFF)
                bytes.Add(value);
        }

    }
}
