using Assets.Scripts.Land;
using Assets.Scripts.Land.Features;
using Assets.Scripts.Land.Features.Implementation;
using Assets.Scripts.Land.Features.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Generator.Implementation
{
    /// <summary>
    /// This is a very basic model for generating populations.
    /// 
    /// A series of random population pockets will be placed. These population pockets will then
    /// be expanded out with random variations, but with diminishing populations.
    /// 
    /// This should provide a slightly more realistic population distribution on a landmass.
    /// </summary>
    class BasicPopulationGeneratorImpl : PopulationGenerator
    {
        // Constants
        private const int LARGE_POPULATION_CHUNKS = 10;
        private const int LARGE_POPULATION_MIN = 300000;
        private const int LARGE_POPULATION_MAX = 500000;
        private const float INITIAL_CHUNK_PCT = 0.05f;
        private const float CHUNK_PCT_DECAY = 0.01f;
        private const float CHUNK_PCT_MIN = 0.01f;
        private const int MAXIMUM_SUBURBS_PER_CITY = 100;
        //private const float RANGE_EXPANSION_FACTOR = 1.01f; // TODO: Rethink, needs to be more smooth

        // Variables
        private Landmass landmass;
        private Demographics demographics;

        // Constructors
        public BasicPopulationGeneratorImpl(Landmass landmass)
        {
            this.landmass = landmass;

            // Create a new demographics
            demographics = new DemographicsImpl();

            // Set default demographics
            foreach (Chunk chunk in landmass.getChunks())
            {
                setPopulation(chunk, 0);
            }
        }

        // Functions
        public Demographics generatePopulation()
        {
            HashSet<Chunk> cityChunks = new HashSet<Chunk>();
            // 1. Randomly choose chunks to represent cities - make sure they are not too close to other chunks
            int chunksChosen = 0;
            while(chunksChosen < LARGE_POPULATION_CHUNKS) {
                // Choose a random chunk
                Chunk randomChunk = landmass.getChunks()[UnityEngine.Random.Range(0, landmass.getChunks().Count - 1)];

                // Make sure it isn't already chosen
                if (cityChunks.Contains(randomChunk))
                {
                    continue;
                }

                // TODO: Make sure it isn't too close to other cities

                // Add it to the list
                cityChunks.Add(randomChunk);

                chunksChosen++;
            }

            List<Suburb> suburbs = new List<Suburb>();
            List<City> cities = new List<City>();

            // 2. Within the cities, spread out suburbs around the cities, spreading them further out the further away from the city they get
            HashSet<Chunk> suburbChunks = new HashSet<Chunk>();
            foreach (Chunk cityChunk in cityChunks)
            {
                // a) Generate a population for the city
                int cityPopulation = UnityEngine.Random.Range(LARGE_POPULATION_MIN, LARGE_POPULATION_MAX);

                // b) Create demographics for the city
                DemographicInfo cityDemographics = new DemographicInfo(cityPopulation);
                List<Suburb> citySuburbs = new List<Suburb>();

                // c) Define a center suburb for the city and find its population
                int cityDecayPopulation = cityPopulation;
                float cityDecayRate = INITIAL_CHUNK_PCT;

                int centerSuburbPopulation = (int)Math.Floor(cityPopulation * cityDecayRate);

                cityDecayPopulation -= centerSuburbPopulation;
                cityDecayRate = Math.Max(cityDecayRate - CHUNK_PCT_DECAY, CHUNK_PCT_MIN);

                Suburb centerSuburb = new Suburb(cityChunk, new List<Chunk> { cityChunk }, new DemographicInfo(centerSuburbPopulation));
                suburbs.Add(centerSuburb);
                suburbChunks.Add(cityChunk);
                citySuburbs.Add(centerSuburb);
                setPopulation(cityChunk, centerSuburbPopulation);

                // d) Expand outwards randomly choosing suburbs for the city until no population is left or maximum suburbs is met
                // TODO: Come up with a more accurate range exit condition
                int range = 1;
                while (cityDecayPopulation > 0 && 
                    citySuburbs.Count < MAXIMUM_SUBURBS_PER_CITY && 
                    !(range > landmass.getChunkCols() && range > landmass.getChunkRows()))
                {
                    // i) Randomly sample along a circle of range, placing suburbs, making sure a suburb doesn't already exist
                    for (int i = 0; i < UnityEngine.Random.Range(3, 5); i++) // TODO: Make this more flexible
                    {
                        // Make sure we haven't run out of population yet
                        if (cityDecayPopulation <= 0)
                        {
                            break;
                        }

                        // TODO: Vary range a bit more with randomness(an example is there)
                        UnityEngine.Vector2 randomRangeCircle = UnityEngine.Random.insideUnitCircle * (range + 0.5f /*+ UnityEngine.Random.Range(0, range * RANGE_EXPANSION_FACTOR)*/);
                        
                        // Get the row/col indexes as deviations from the city chunk index
                        int rowIndex = (int)Math.Round(randomRangeCircle.x) + cityChunk.getRowIndex();
                        int colIndex = (int)Math.Round(randomRangeCircle.y) + cityChunk.getColIndex();

                        // Check if they are outside of the boundaries, and if so ignore
                        if (rowIndex < 0 || rowIndex > landmass.getChunkRows() - 1 || colIndex < 0 || colIndex > landmass.getChunkCols() - 1)
                        {
                            continue;
                        }

                        // Get the chunk and make sure it hasn't already been chosen
                        Chunk suburbChunk = landmass.getChunk(rowIndex, colIndex);
                        if (suburbChunks.Contains(suburbChunk))
                        {
                            continue;
                        }

                        // Determine the population that the suburb gets
                        int suburbPopulation = (int)Math.Floor(cityPopulation * cityDecayRate);

                        cityDecayPopulation -= suburbPopulation;
                        cityDecayRate = Math.Max(cityDecayRate - CHUNK_PCT_DECAY, CHUNK_PCT_MIN);

                        Suburb suburb = new Suburb(suburbChunk, new List<Chunk> { suburbChunk }, new DemographicInfo(suburbPopulation));
                        suburbs.Add(suburb);
                        suburbChunks.Add(suburbChunk);
                        citySuburbs.Add(suburb);
                        setPopulation(suburbChunk, suburbPopulation);
                    }

                    // ii) Expand the range
                    range = getNextRange(range);
                }

                // e) Create the city
                City city = new City(centerSuburb, citySuburbs, cityDemographics);
                cities.Add(city);
            }

            // 3. Add the suburbs and cities
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

        /// <summary>
        /// Gets the next range based on the previous range.
        /// </summary>
        /// <param name="previousRange">The previous range.</param>
        /// <returns>The next range</returns>
        private int getNextRange(int previousRange)
        {
            // TODO: Put this into settings somehow
            float distance = convertRangeToDistance(previousRange);

            // For now, we will judge by a proportion of its length. Change to use realistic
            // size scaling later on.
            float lengthProp = landmass.getLength() / distance;

            // Define three phases of growth
            if (lengthProp <= 0.05f)
            {
                return previousRange + 5;
            }
            else if (lengthProp <= 0.10f)
            {
                return previousRange + 10;
            }
            else
            {
                return previousRange + 30;
            }
        }

        /// <summary>
        /// Converts a given range into a distance based off the land mass.
        /// </summary>
        /// <param name="range">The range to convert.</param>
        /// <returns>The distance equivilant of the range in the context of the landmass.</returns>
        private float convertRangeToDistance(int range)
        {
            return range * landmass.getChunkSize();
        }

        /// <summary>
        /// Sets the population for a chunk in the demographics.
        /// </summary>
        /// <param name="chunk">The chunk to set the population for.</param>
        /// <param name="population">The population to set it to.</param>
        private void setPopulation(Chunk chunk, int population)
        {
            demographics.setDemographics(chunk, new DemographicInfo(population));
        }

        /// <summary>
        /// Adds population to an existing chunk.
        /// </summary>
        /// <param name="chunk">The chunk to add population to.</param>
        /// <param name="population">The population to add.</param>
        private void addPopulation(Chunk chunk, int population)
        {
            DemographicInfo chunkInfo = demographics.getDemographics(chunk);

            setPopulation(chunk, chunkInfo.getPopulation() + population);
        }

        /// <summary>
        /// Averages the population of an existing chunk if it is not zero.
        /// </summary>
        /// <param name="chunk">The chunk to average population to.</param>
        /// <param name="population">The population to average.</param>
        private void averagePopulation(Chunk chunk, int population)
        {
            DemographicInfo chunkInfo = demographics.getDemographics(chunk);

            if (chunkInfo.getPopulation() <= 0)
            {
                setPopulation(chunk, population);
            }
            else
            {
                setPopulation(chunk, (int)Math.Round((chunkInfo.getPopulation() + population) / 2.0f));
            }
        }

        /// <summary>
        /// Gets the population of the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk to get the population for.</param>
        /// <returns>The population of the chunk.</returns>
        private int getPopulation(Chunk chunk)
        {
            return demographics.getDemographics(chunk).getPopulation();
        }
    }
}
