using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Land.Features
{
    public enum TerrainType { Grass };

    public interface Terrain
    {

        /* Gets the type of terrain for this chunk.
         * 
         * @param chunk The chunk to get the terrain type for.
         * @return TerrainType The terrain type for the chunk.
         */
        TerrainType getTerrainType(Chunk chunk);

        /* Sets the terrain type for the specified chunk.
         * 
         * @param chunk The chunk to set the terrain for.
         * @param type The type of terrain.
         */
        void setTerrainType(Chunk chunk, TerrainType type);
    }
}
