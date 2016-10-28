using Assets.Scripts.Land;
using Assets.Scripts.Land.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Generator
{
    public interface TerrainGenerator
    {
        /* Generates the terrain based off of the landmass
         * 
         * @returns Terrain The terrain generated off of the landmass.
         */
        Terrain generateTerrain();

        Landmass getLandmass();
    }
}
