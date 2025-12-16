using netDxf;
using netDxf.Entities;
using System.IO;

namespace AresLitho.Services.Dxf
{
    internal static class R12DxfLoader
    {
        public static DxfDocument Load(string path)
        {
            var lines = File.ReadAllLines(path);
            DxfDocument dxf = new();

            var records = ParseRecords(lines).ToList();

            bool inEntitiesSection = false;
            string currentEntityType = string.Empty;

            var entityData = new Dictionary<int, string>();

            foreach (var (code, value) in records)
            {
                if (code == 0 || code == 2)
                {
                    if (value == "SECTION")
                    {
                        continue;
                    }
                    else if (value == "ENDSEC")
                    {
                        // Add the last entity if exists
                        if (inEntitiesSection && entityData.Count > 0 && !string.IsNullOrEmpty(currentEntityType))
                        {
                            var entity = ParseEntity(currentEntityType, entityData);
                            if (entity != null)
                            {
                                dxf.Entities.Add(entity);
                            }
                        }
                        inEntitiesSection = false;
                        continue;
                    }
                    else if (value == "ENTITIES")
                    {
                        inEntitiesSection = true;
                        continue;
                    }
                    else if (inEntitiesSection)
                    {
                        if (entityData.Count > 0 && !string.IsNullOrEmpty(currentEntityType))
                        {
                            var entity = ParseEntity(currentEntityType, entityData);
                            if (entity != null)
                            {
                                dxf.Entities.Add(entity);
                            }
                            entityData.Clear();
                        }
                        currentEntityType = value;
                    }
                }
                else if (inEntitiesSection)
                {
                    entityData[code] = value;
                }
            }

            return dxf;
        }

        private static IEnumerable<(int Code, string Value)> ParseRecords(string[] lines)
        {
            for (int i = 0; i < lines.Length; i += 2)
            {
                if (int.TryParse(lines[i], out int code))
                {
                    yield return (code, lines[i + 1]);
                }
                else
                {
                    throw new FormatException($"Invalid DXF code at line {i + 1}: {lines[i]}");
                }
            }
        }

        private static EntityObject? ParseEntity(string type, Dictionary<int, string> data)
        {
            switch (type)
            {
                case "LINE":
                    return new Line(
                        new Vector3(
                            double.Parse(data[10]),
                            double.Parse(data[20]),
                            double.Parse(data.GetValueOrDefault(30, "0"))
                        ),
                        new Vector3(
                            double.Parse(data[11]),
                            double.Parse(data[21]),
                            double.Parse(data.GetValueOrDefault(31, "0"))
                        )
                    );
                case "CIRCLE":
                    return new Circle(
                        new Vector3(
                            double.Parse(data[10]),
                            double.Parse(data[20]),
                            double.Parse(data.GetValueOrDefault(30, "0"))
                        ),
                        double.Parse(data[40])
                    );
                case "TEXT":
                    return new Text(
                        data[1],
                        new Vector3(
                            double.Parse(data[10]),
                            double.Parse(data[20]),
                            double.Parse(data.GetValueOrDefault(30, "0"))
                        ),
                        double.Parse(data[40])
                    );
                case "LWPOLYLINE":
                    var polyline = new Polyline2D();
                    int vertexIndex = 0;
                    while (data.ContainsKey(10 + vertexIndex * 10))
                    {
                        polyline.Vertexes.Add(new Polyline2DVertex(
                            double.Parse(data[10 + vertexIndex * 10]),
                            double.Parse(data[20 + vertexIndex * 10])
                        ));
                        vertexIndex++;
                    }
                    return polyline;
                default:
                    return null;
            }
        }
    }
}
