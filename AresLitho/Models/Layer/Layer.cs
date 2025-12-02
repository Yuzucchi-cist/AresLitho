using AresLitho.Models.ImportedFiles;
using AresLitho.Models.ImportedFiles.PCBDxf;
using AresLitho.Models.ImportedFiles.Stl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AresLitho.Models.Layer
{
    internal abstract class Layer
    {
        public string Name { get; set; }
        public BinImage BinImage { get; }

        public Layer(string name, BinImage binImage)
        {
            Name = name;
            BinImage = binImage;
        }

        public static List<Layer> Load(List<ImportedFile> importedFiles)
        {
            if (importedFiles.All(file => file is DxfFile))
            {
                List<DxfFile> dxfFiles = importedFiles.Cast<DxfFile>().ToList();
                return Load(dxfFiles);
            }
            else if (importedFiles.All(file => file is StlFile))
            {
                List<StlFile> stlFiles = importedFiles.Cast<StlFile>().ToList();
                return Load(stlFiles);
            }
            else
            {
                throw new Exception("Unsupported file type.");
            }
        }

        public static List<Layer> Load(List<DxfFile> dxfFiles)
        {
            ValidateFileName(dxfFiles);

            if (dxfFiles.Count == 1)
            {
                switch (dxfFiles[0])
                {
                    case CuLayer cuLayer:
                        return new List<Layer> { new SingleLayer(cuLayer) };
                    case PasteLayer pasteLayer:
                        return new List<Layer> { new SingleLayer(pasteLayer) };
                    case EdgeCutLayer edgeCutLayer:
                        return new List<Layer> { new SingleLayer(edgeCutLayer) };
                    default:
                        throw new Exception("Unknown layer type.");
                }
            }

            var (edgeCut, frontPaste, frontCu, innerCu, backCu, backPaste) = ParseLayers(dxfFiles);

            List<Layer> layers = [];

            FilmLayer frontFilmLayer = new(edgeCut, frontPaste);
            FilmLayer backFilmLayer = new(edgeCut, backPaste);
            MetalLayer frontMetalLayer = new(frontCu);
            MetalLayer backMetalLayer = new(backCu);

            layers.Add(frontFilmLayer);
            layers.Add(frontMetalLayer);

            if (innerCu.Count > 0)
            {
                layers.Add(new InnerFilmLayer(edgeCut, frontCu, innerCu.First()));
                layers.Add(new MetalLayer(innerCu.First()));

                for (int i = 1; i < innerCu.Count - 1; i++)
                {
                    layers.Add(new InnerFilmLayer(edgeCut, innerCu[i - 1], innerCu[i]));
                    layers.Add(new MetalLayer(innerCu[i]));
                }

                layers.Add(new InnerFilmLayer(edgeCut, innerCu.Last(), backCu));
                layers.Add(new MetalLayer(innerCu.Last()));
            }

            layers.Add(backMetalLayer);
            layers.Add(backFilmLayer);
            return layers;
        }

        private static List<Layer> Load(List<StlFile> stlFiles)
        {
            return stlFiles.Select(stlFile => new SingleLayer(stlFile)).ToList<Layer>();
        }

        private static void ValidateFileName(List<DxfFile> dxfFiles)
        {
            if (dxfFiles.Count == 1)
                return;

            int cuCount = 0, pasteCount = 0, edgeCutCount = 0;
            foreach (var file in dxfFiles)
            {
                switch (file)
                                    {
                    case CuLayer:
                        cuCount++;
                        break;
                    case PasteLayer:
                        pasteCount++;
                        break;
                    case EdgeCutLayer:
                        edgeCutCount++;
                        break;
                    default:
                        throw new Exception($"Unknown layer type in file: {file.FileName}");
                }
            }
            if (cuCount != 2 && pasteCount != 2 && edgeCutCount != 1)
            {
                throw new Exception("Invalid number of layer files provided.");
            }
        }

        private static (EdgeCutLayer edgeCut, PasteLayer f_paste, CuLayer f_cu, List<CuLayer> in_cu, CuLayer f_back, PasteLayer b_paste) ParseLayers(List<DxfFile> dxfFiles)
        {
            List<CuLayer> cuLayers = dxfFiles.OfType<CuLayer>().ToList();

            CuLayer? frontCu = cuLayers.FirstOrDefault(layer => layer.LayerSide == CuLayerSide.Front);
            CuLayer? backCu = cuLayers.FirstOrDefault(layer => layer.LayerSide == CuLayerSide.Back);
            List<CuLayer> innerCu = cuLayers.Where(layer => layer.LayerSide == CuLayerSide.Inner).ToList();

            List<PasteLayer> pasteLayers = dxfFiles.OfType<PasteLayer>().ToList();
            PasteLayer? frontPaste = pasteLayers.FirstOrDefault(layer => layer.LayerSide == PasteLayerSide.Front);
            PasteLayer? backPaste = pasteLayers.FirstOrDefault(layer => layer.LayerSide == PasteLayerSide.Back);

            EdgeCutLayer? edgeCut = dxfFiles.OfType<EdgeCutLayer>().FirstOrDefault();

            if (frontCu is null || backCu is null || frontPaste is null || backPaste is null || edgeCut is null)
                throw new Exception("Missing required layers");

            return (edgeCut, frontPaste, frontCu, innerCu, backCu, backPaste);
        }
    }
}
