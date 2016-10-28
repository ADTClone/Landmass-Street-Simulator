using Assets.Scripts.Land;
using Assets.Scripts.Land.Features;
using Assets.Scripts.Land.Features.Implementation;
using Assets.Scripts.Land.Features.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Generator.Prototype
{
    class StubPopulationGenerator : PopulationGenerator
    {
        // Variables
        private Landmass landmass;

        // Constructors
        public StubPopulationGenerator(Landmass landmass)
        {
            this.landmass = landmass;
        }

        // Functions
        public Demographics generatePopulation()
        {
            // Create a new demographics
            Demographics demographics = new DemographicsImpl();

            // Populate the demographics with some random values
            foreach(Chunk chunk in landmass.getChunks()) {
                DemographicInfo info = new DemographicInfo(UnityEngine.Random.Range(0, 10000));
                demographics.setDemographics(chunk, info);
            }

            // Return the demographics
            return demographics;
        }

        public Landmass getLandmass()
        {
            return landmass;
        }
    }
}
