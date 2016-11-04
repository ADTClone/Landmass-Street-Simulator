using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Land.Features.Structs
{
    /// <summary>
    /// Represents a suburb(which can include a bigger population suburb like a city
    /// or a town).
    /// </summary>
    public class Suburb
    {
        // Variables
        private Chunk center;
        private List<Chunk> chunks;
        private DemographicInfo demographics;

        // Constructors
        public Suburb(Chunk center, List<Chunk> chunks, DemographicInfo demographics)
        {
            this.center = center;
            this.chunks = chunks;
            this.demographics = demographics;
        }

        // Functions
        /// <summary>
        /// Gets the chunk that represents the center of the suburb.
        /// </summary>
        /// <returns>The center of the suburb.</returns>
        public Chunk getCenter()
        {
            return center;
        }

        /// <summary>
        /// Gets all the chunks contained within the suburb.
        /// </summary>
        /// <returns>All the chunks within the suburb.</returns>
        public List<Chunk> getChunks()
        {
            return chunks;
        }

        /// <summary>
        /// Gets the demographics information of the suburb.
        /// </summary>
        /// <returns>The demographics information of the suburb.</returns>
        public DemographicInfo getDemographics()
        {
            return demographics;
        }
    }
}
