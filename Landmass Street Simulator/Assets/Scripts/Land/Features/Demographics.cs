using Assets.Scripts.Land.Features.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Land.Features
{
    public interface Demographics
    {
        /* Gets the demographic information for a chunk */
        DemographicInfo getDemographics(Chunk chunk);

        /// <summary>
        /// Gets a list of cities.
        /// </summary>
        /// <returns>A list of high population suburbs.</returns>
        List<City> getCities();

        /// <summary>
        /// Gets a list of all suburbs.
        /// </summary>
        /// <returns>A list of standard suburbs.</returns>
        List<Suburb> getSuburbs();

        /* Sets the demographics information for a chunk */
        void setDemographics(Chunk chunk, DemographicInfo demographicsInfo);

        /// <summary>
        /// Adds a city to the demographics.
        /// </summary>
        /// <param name="city">The city to add.</param>
        void addCity(City city);

        /// <summary>
        /// Adds a suburb to the demographics.
        /// </summary>
        /// <param name="suburb">The suburb to add.</param>
        void addSuburb(Suburb suburb);
        
        /// <summary>
        /// Checks whether the chunk is a city.
        /// </summary>
        /// <param name="chunk">The chunk to check.</param>
        /// <returns></returns>
        bool isCity(Chunk chunk);

        /// <summary>
        /// Checks whether the chunk is a suburb.
        /// </summary>
        /// <param name="chunk">The chunk to check.</param>
        /// <returns></returns>
        bool isSuburb(Chunk chunk);
    }
}
