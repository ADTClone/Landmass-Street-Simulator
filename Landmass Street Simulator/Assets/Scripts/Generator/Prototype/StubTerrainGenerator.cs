using Assets.Scripts.Land;
using Assets.Scripts.Land.Features;
using Assets.Scripts.Land.Features.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Generator.Prototype
{
    class StubTerrainGenerator : TerrainGenerator
    {
        // Variables
        private Landmass landmass;

        // Constructors
        public StubTerrainGenerator(Landmass landmass)
        {
            this.landmass = landmass;
        }

        // Functions
        /* Generates some fake, flat terrain */
        public Terrain generateTerrain()
        {
            // Create some new terrain
            Terrain terrain = new TerrainImpl();

            // Lots of grass
            foreach (Chunk chunk in landmass.getChunks())
            {
                terrain.setTerrainType(chunk, TerrainType.Grass);
            }

            // Return the terrain
            return terrain;
        }

        public Landmass getLandmass()
        {
            return landmass;
        }
    }
}
