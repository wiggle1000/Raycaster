using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal static class LevelSerializerUtils
    {
        public static string levelDir;

        static LevelSerializerUtils()
        {
            levelDir = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Maps");
            Console.ForegroundColor = ConsoleColor.Red;
            if(OperatingSystem.IsWindows())
                Console.SetWindowSize(128, 8);
        }

        private static MapLayer ReadLayer(BinaryReader reader, int levelW, int levelH)
        {
            MapLayer layer = new MapLayer(levelW, levelH);
            for (int x = 0; x < levelW; x++)
            {
                for (int y = 0; y < levelH; y++)
                {
                    layer.SetAt(x, y, reader.Read7BitEncodedInt());
                }
            }
            return layer;
        }

        private static void WriteLayer(BinaryWriter writer, MapLayer layer)
        {
            for (int x = 0; x < layer.width; x++)
            {
                for (int y = 0; y < layer.height; y++)
                {
                    writer.Write7BitEncodedInt(layer.GetAt(x, y));
                }
            }
        }

        public static LevelImplementation LoadLevel(string fileName)
        {
            string levelPath = fileName;
            Debug.Log("LOADING LEVEL. " + fileName);
            //string levelPath = Path.Combine(levelDir, fileName);
            /*if (!Directory.Exists(levelDir))
            {
                Debug.Log("LEVEL FOLDER MISSING!");
                return null;
            }*/
            if (!File.Exists(levelPath))
            {
                Debug.Log("LEVEL NOT FOUND AT: " + levelPath);
                return null;
            }
            using (FileStream fs = File.Open(levelPath, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(fs, Encoding.UTF8))
                {
                    int levelW = reader.ReadInt32();
                    int levelH = reader.ReadInt32();
                    LevelImplementation level = new LevelImplementation(levelW, levelH);
                    level.levelName = reader.ReadString();
                    reader.ReadString(); reader.ReadString(); reader.ReadString(); //placeholder strings
                    level.floorLayer = ReadLayer(reader, levelW, levelH);
                    level.wallLayer = ReadLayer(reader, levelW, levelH);
                    level.ceilLayer = ReadLayer(reader, levelW, levelH);

                    level.markerLayer = ReadLayer(reader, levelW, levelH);
                    level.metaLayer = ReadLayer(reader, levelW, levelH);
                    return level;
                }
            }
        }
        public static void SaveLevel(LevelImplementation level, string fileName)
        {
            Debug.Log("SAVING LEVEL. " + fileName);
            string levelPath = fileName;
            if (File.Exists(levelPath))
            {
                Debug.Log("LEVEL EXISTS AT: " + levelPath);
                return;
            }
            using (FileStream fs = File.Open(levelPath, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(fs, Encoding.UTF8))
                {
                    writer.Write(level.width);
                    writer.Write(level.height);
                    writer.Write(level.levelName);
                    writer.Write(""); writer.Write(""); writer.Write(""); //placeholder strings

                    WriteLayer(writer, level.floorLayer);
                    WriteLayer(writer, level.wallLayer);
                    WriteLayer(writer, level.ceilLayer);

                    WriteLayer(writer, level.markerLayer);
                    WriteLayer(writer, level.metaLayer);
                }
                fs.Close();
            }
        }
    }
}
