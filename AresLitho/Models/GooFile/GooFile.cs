using AresLitho.Commons;
using System.IO;
using System.Text;

namespace AresLitho.Models.GooFile
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
            Header.TransitionLayers = (short)Layers.Count;
        }
        public static GooFile CreateFromBinImageToCenter(BinImage layerImage)
        {
            GooFile goo = new()
            {
                Layers = [],
                Header = GooHeaderInfo.CreateDefaultGooHeaderInfo()
            };

            goo.Header.SmallPreviewImage = layerImage.Scale(GooHeaderInfo.SmallPreviewImageWidth, GooHeaderInfo.SmallPreviewImageHeight).EncodeToBinary();
            goo.Header.BigPreviewImage = layerImage.Scale(GooHeaderInfo.BigPreviewImageWidth, GooHeaderInfo.BigPreviewImageHeight).EncodeToBinary();
            goo.Header.TransitionLayers = 1;

            layerImage = new BinImage(goo.Header.XResolution, goo.Header.YResolution).SetToCenter(layerImage);

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

            goo.Header.SmallPreviewImage = layerImage.Scale(GooHeaderInfo.SmallPreviewImageWidth, GooHeaderInfo.SmallPreviewImageHeight).EncodeToBinary();
            goo.Header.BigPreviewImage = layerImage.Scale(GooHeaderInfo.BigPreviewImageWidth, GooHeaderInfo.BigPreviewImageHeight).EncodeToBinary();
            goo.Header.TransitionLayers = 1;

            goo.Layers.Add(new GooLayerContent(layerImage.Scale(goo.Header.XResolution, goo.Header.YResolution)));

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
            Header.WriteToStream(outputStream);
            foreach (var layer in Layers)
            {
                layer.WriteToStream(outputStream);
            }
            
            using BigEndianBinaryWriter bw = new(outputStream, Encoding.ASCII, true);
            bw.Write([0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x44, 0x4C, 0x50, 0x00]);   // Write Ending string
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
        /// 2 次元のグレースケール画像データを Goo 仕様の簡易 RLE（ランレングス）でエンコードします。
        /// エンコードの流れ:
        /// - 左→右、上→下 に走査し各ピクセル値をラン長圧縮する
        /// - 出力先の先頭にマジックバイト 0x55 を追加する
        /// - チャンクは「全 0x00」「全 0xFF」「固定値」のいずれかで表現する
        /// - ラン長は仕様に基づく短・拡張フォーマットを使い、実装上は安全のため 2 バイトラン長までを扱う
        /// - 最後に 8bit チェックサム（先頭の 0x55 を除くバイトの合計のビット反転）を追加する
        /// </summary>
        /// <param name="data">エンコード対象の 2 次元バイト配列。縦が行、高さ、横が列、幅。null の場合は例外を投げます。</param>
        /// <returns>Goo 用に RLE エンコードされたバイト配列（先頭に 0x55、末尾にチェックサムを含む）。</returns>
        /// <exception cref="ArgumentNullException">`data` が null の場合にスローされます。</exception>
        public static byte[] EncodeImageDataFromBinaryWithRunLength(byte[,] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

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
                        if (run >= 0xFFF) // セーフティ制限
                        // CHITO BOXで3バイトラン長、4バイトラン長が実装されていないため、2バイトラン長でセーフティ制限
                        // if (run >= 0xFFFF) // セーフティ制限
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

            // チェックサム（0x55以外のバイトの総和 8bitのビット反転）
            int checksum = 0;
            for (int i = 1; i < bytes.Count; i++)
                checksum += bytes[i];
            checksum = ~checksum & 0xff;


            bytes.Add((byte)checksum);

            return [.. bytes];
        }

        /// <summary>
        /// 値とラン長から RLE チャンクを追加
        /// （全0x00 / 全0xFF / 固定値対応）
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

            // --- ラン長符号化 ---
            if (runLength < 16)
            {
                // 4bitラン長
                byte b0 = (byte)(typeBits | (runLength & 0x0F));
                bytes.Add(b0);
            }
            // CHITO BOXで3バイトラン長、4バイトラン長が実装されていないため、2バイトラン長でセーフティ制限
            //else if (run < 4096)
            else
            {
                // 2バイトラン長 (byte0[5:4]=01)
                byte b0 = (byte)(typeBits | 0b0001_0000 | (runLength & 0x0F));
                byte b1 = (byte)((runLength >> 4) & 0xFF);
                bytes.Add(b0);
                bytes.Add(b1);
            }

            // CHITO BOXで3バイトラン長、4バイトラン長が実装されていないため、2バイトラン長でセーフティ制限
            /*
            else if (run < 1048576)
            {
                // 3バイトラン長 (byte0[5:4]=10)
                byte b0 = (byte)(typeBits | 0b0010_0000 | (runLength & 0x0F));
                byte b1 = (byte)((runLength >> 4) & 0xFF);
                byte b2 = (byte)((runLength >> 12) & 0xFF);
                bytes.Add(b0);
                bytes.Add(b1);
                bytes.Add(b2);
            }
            else
            {
                // 4バイトラン長 (byte0[5:4]=11)
                byte b0 = (byte)(typeBits | 0b0011_0000 | (run & 0x0F));
                byte b1 = (byte)((runLength >> 4) & 0xFF);
                byte b2 = (byte)((runLength >> 12) & 0xFF);
                byte b3 = (byte)((runLength >> 20) & 0xFF);
                bytes.Add(b0);
                bytes.Add(b1);
                bytes.Add(b2);
                bytes.Add(b3);
            }
            */

            // --- 固定値チャンクの場合、値を追加 ---
            if (value != 0x00 && value != 0xFF)
                bytes.Add(value);
        }
    }
}
