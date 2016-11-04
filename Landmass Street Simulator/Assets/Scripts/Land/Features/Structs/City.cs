using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Land.Features.Structs
{
    /// <summary>
    /// A city contains a collection of suburbs.
    /// </summary>
    public class City
    {
        // Variables
        private Suburb center;
        private List<Suburb> suburbs;
        private DemographicInfo demographics;

        // Constructors
        public City(Suburb center, List<Suburb> suburbs, DemographicInfo demographics)
        {
            this.center = center;
            this.suburbs = suburbs;
            this.demographics = demographics;
        }

        // Functions
        /// <summary>
        /// Gets the "center suburb" of the city.
        /// </summary>
        /// <returns>The center suburb of the city.</returns>
        public Suburb getCenter()
        {
            return center;
        }

        /// <summary>
        /// Gets the suburbs contained within the city.
        /// </summary>
        /// <returns>The suburbs contained within the city.</returns>
        public List<Suburb> getSuburbs()
        {
            return suburbs;
        }

        /// <summary>
        /// Gets the demographics info of the city.
        /// </summary>
        /// <returns>The demographics info of the city.</returns>
        public DemographicInfo getDemographics()
        {
            return demographics;
        }
    }
}
