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
        private const int LARGE_POPULATION_CHUNKS = 100;
        private const int LARGE_POPULATION_MIN = 50000;
        private const int LARGE_POPULATION_MAX = 50000;
        private const int POPULATION_LOWER_LIMIT = 100; // The limit at which spreading stops
        private const float POPULATION_DECAY_RATE = 0.97f;
        private const float POPULATION_DECAY_RATE_VARIANCE = 0.01f;
        private const float INITIAL_DECAY_RATE = 0.7f;
        private const float POPULATION_LOCAL_VARIANCE = 0.1f;
        private const int RANGE_INCREMENT = 1;

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
            // TODO: Something seems to be keeping the population stuck in an area on bigger sizes
            // Randomly choose a few chunks to place "large" population centers
            HashSet<Chunk> largePopulationChunks = new HashSet<Chunk>();
            for (int i = 0; i < LARGE_POPULATION_CHUNKS; i++)
            {
                // Choose a random chunk
                Chunk randomChunk = landmass.getChunks()[UnityEngine.Random.Range(0, landmass.getChunks().Count - 1)];

                // Set its population to a random value
                setPopulation(randomChunk, UnityEngine.Random.Range(LARGE_POPULATION_MIN, LARGE_POPULATION_MAX));

                // Add it to a list
                largePopulationChunks.Add(randomChunk);
            }

            // Recursively spread out the population from these chunks until they run out of population to distribute
            foreach (Chunk chunk in largePopulationChunks)
            {
                float currentPopulation = getPopulation(chunk) * INITIAL_DECAY_RATE;
                int chunkRowIndex = chunk.getRowIndex();
                int chunkColIndex = chunk.getColIndex();

                // Loop in expanding ranges, spreading the population until it completely decays
                int range = 1;
                while (currentPopulation > POPULATION_LOWER_LIMIT)
                {
                    // Loop through rows
                    for (int rowIndex = chunkRowIndex - range; rowIndex <= chunkRowIndex + range; rowIndex++)
                    {
                        // Make sure it isn't off the edge of the area
                        if (rowIndex < 0 || rowIndex > landmass.getChunkRows() - 1)
                        {
                            continue;
                        }

                        // Bottom side
                        int colIndex = chunkColIndex - range;
                        if (!(colIndex < 0 || colIndex > landmass.getChunkCols() - 1))
                        {
                            // We can spread the population now
                            Chunk spreadTo = landmass.getChunk(rowIndex, colIndex);
                            int spreadToPopulation = (int)Math.Floor(UnityEngine.Random.Range(currentPopulation * (1.0f - POPULATION_LOCAL_VARIANCE), 
                                                                        currentPopulation * (1.0f + POPULATION_LOCAL_VARIANCE)));

                            // Spread it
                            addPopulation(spreadTo, spreadToPopulation);
                        }

                        // Top side
                        colIndex = chunkColIndex + range;
                        if (!(colIndex < 0 || colIndex > landmass.getChunkCols() - 1))
                        {
                            // We can spread the population now
                            Chunk spreadTo = landmass.getChunk(rowIndex, colIndex);
                            int spreadToPopulation = (int)Math.Floor(UnityEngine.Random.Range(currentPopulation * (1.0f - POPULATION_LOCAL_VARIANCE),
                                                                        currentPopulation * (1.0f + POPULATION_LOCAL_VARIANCE)));

                            // Spread it
                            addPopulation(spreadTo, spreadToPopulation);
                        }

                    }

                    // Loop through cols(excluding diagonals)
                    for (int colIndex = chunkColIndex - range + 1; colIndex <= chunkColIndex + range - 1; colIndex++)
                    {
                        // Make sure it isn't off the edge of the area
                        if (colIndex < 0 || colIndex > landmass.getChunkRows() - 1)
                        {
                            continue;
                        }

                        // Left side
                        int rowIndex = chunkRowIndex - range;

                        if (!(rowIndex < 0 || rowIndex > landmass.getChunkRows() - 1))
                        {
                            // We can spread the population now
                            Chunk spreadTo = landmass.getChunk(rowIndex, colIndex);
                            int spreadToPopulation = (int)Math.Floor(UnityEngine.Random.Range(currentPopulation * (1.0f - POPULATION_LOCAL_VARIANCE), 
                                                                        currentPopulation * (1.0f + POPULATION_LOCAL_VARIANCE)));

                            // Spread it
                            addPopulation(spreadTo, spreadToPopulation);
                        }

                        // Right side
                        rowIndex = chunkRowIndex + range;

                        if (!(rowIndex < 0 || rowIndex > landmass.getChunkRows() - 1))
                        {
                            // We can spread the population now
                            Chunk spreadTo = landmass.getChunk(rowIndex, colIndex);
                            int spreadToPopulation = (int)Math.Floor(UnityEngine.Random.Range(currentPopulation * (1.0f - POPULATION_LOCAL_VARIANCE),
                                                                        currentPopulation * (1.0f + POPULATION_LOCAL_VARIANCE)));

                            // Spread it
                            addPopulation(spreadTo, spreadToPopulation);
                        }

                    }

                    // Increase the range
                    range += RANGE_INCREMENT;

                    // Decay the population
                    currentPopulation *= (UnityEngine.Random.Range(POPULATION_DECAY_RATE - POPULATION_DECAY_RATE_VARIANCE, POPULATION_DECAY_RATE + POPULATION_DECAY_RATE_VARIANCE));
                }
            }

            // Return the demographics
            return demographics;
        }

        public Landmass getLandmass()
        {
            return landmass;
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
