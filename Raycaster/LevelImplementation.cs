using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal class LevelImplementation
    {
        public string levelName = "UNNAMED LEVEL";

        public Entity[] entities;

        public int width, height = 32;

        public MapLayer floorLayer;
        public MapLayer wallLayer;
        public MapLayer ceilLayer;

        public MapLayer markerLayer; //entity spawns, spawn positions, etc
        public MapLayer metaLayer; //special information for tile types

        public enum LayerType
        {
            floor = 0,
            wall = 1,
            ceil = 2,
            marker = 3,
            meta = 4
        }


        public LevelImplementation(int w = 32, int h = 32)
        {
            width = w;
            height = h;
            InitLayers();
        }
        public static LevelImplementation LoadLevel(string fileName)
        {
            LevelImplementation lvl = LevelSerializerUtils.LoadLevel(fileName);
            return lvl;
        }

        private void InitLayers()
        {
            wallLayer = new MapLayer(width, height);
            ceilLayer = new MapLayer(width, height);
            floorLayer = new MapLayer(width, height);

            markerLayer = new MapLayer(width, height);
            metaLayer = new MapLayer(width, height);
        }

        public ref MapLayer GetMapLayerByLayerType(LayerType lt)
        {
            switch (lt)
            {
                case LayerType.floor: return ref floorLayer;
                case LayerType.wall: return ref wallLayer;
                case LayerType.ceil: return ref ceilLayer;
                case LayerType.marker: return ref markerLayer;
                case LayerType.meta: return ref metaLayer;
                default:
                    return ref floorLayer;
            }
        }

        public void TickEntities(float dt)
        {
            foreach(Entity e in entities)
                if(e is ITickable et) et.Tick(dt);
        }
    }
}
