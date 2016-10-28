using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Land.Features;

namespace Assets.Scripts.Land.Features.Implementation
{
    class TerrainImpl : Terrain
    {
        // Variables
        private Dictionary<Chunk, TerrainType> terrainTypes;

        // Constructors
        public TerrainImpl()
        {
            terrainTypes = new Dictionary<Chunk, TerrainType>();
        }

        // Functions
        public TerrainType getTerrainType(Chunk chunk)
        {
            return terrainTypes[chunk];
        }

        public void setTerrainType(Chunk chunk, TerrainType type)
        {
            terrainTypes.Add(chunk, type);
        }
    }
}
