using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Land.Features.Structs
{
    /* Contains information about demographics within a chunk or small unit */
    public class DemographicInfo
    {
        // Variables
        private int population;

        // Constructors
        public DemographicInfo(int population)
        {
            this.population = population;
        }

        // Functions
        public int getPopulation()
        {
            return population;
        }
    }
}
