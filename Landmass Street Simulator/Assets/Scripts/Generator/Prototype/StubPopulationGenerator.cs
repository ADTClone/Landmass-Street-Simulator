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
            List<City> cities = new List<City>();
            List<Suburb> suburbs = new List<Suburb>();

            // Create some random cities
            for (int i = 0; i < 10; i++)
            {
                int randomPopulation = UnityEngine.Random.Range(100000, 400000);
                DemographicInfo cityDemographics = new DemographicInfo(randomPopulation);

                // Create the center suburb
                int centerSuburbPopulation = Mathf.Min(randomPopulation, UnityEngine.Random.Range(1000, 5000));
                randomPopulation -= centerSuburbPopulation;

                Suburb centerSuburb = new Suburb(landmass.getChunks()[UnityEngine.Random.Range(0, landmass.getChunks().Count - 1)], 
                    new List<Chunk>(), 
                    new DemographicInfo(centerSuburbPopulation));

                // Create other random suburbs for the city
                List<Suburb> citySuburbs = new List<Suburb>();
                citySuburbs.Add(centerSuburb);
                suburbs.Add(centerSuburb);

                while (randomPopulation > 0)
                {
                    int suburbPopulation = Mathf.Min(randomPopulation, UnityEngine.Random.Range(1000, 5000));

                    Suburb suburb = new Suburb(landmass.getChunks()[UnityEngine.Random.Range(0, landmass.getChunks().Count - 1)],
                        new List<Chunk>(),
                        new DemographicInfo(suburbPopulation));

                    citySuburbs.Add(suburb);
                    suburbs.Add(suburb);

                    randomPopulation -= centerSuburbPopulation;
                }

                // Create the city
                City city = new City(centerSuburb, citySuburbs, cityDemographics);
                cities.Add(city);
            }


            // Populate the demographics with some random values
            foreach (Chunk chunk in landmass.getChunks())
            {
                DemographicInfo info = new DemographicInfo(UnityEngine.Random.Range(0, 10000));
                demographics.setDemographics(chunk, info);
            }

            // Add the cities and suburbs
            foreach (City city in cities)
            {
                demographics.addCity(city);
            }

            foreach (Suburb suburb in suburbs)
            {
                demographics.addSuburb(suburb);
            }

            UnityEngine.Debug.Log("Total suburbs: " + suburbs.Count);

            // Return the demographics
            return demographics;
        }

        public Landmass getLandmass()
        {
            return landmass;
        }
    }
}
