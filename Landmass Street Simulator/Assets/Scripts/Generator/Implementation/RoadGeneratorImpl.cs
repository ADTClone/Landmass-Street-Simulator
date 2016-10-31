using Assets.Scripts.Land;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Generator.Implementation
{
    class RoadGeneratorImpl : RoadGenerator
    {
        // Variables
        private Landmass landmass;

        // Constructors
        public RoadGeneratorImpl(Landmass landmass)
        {
            this.landmass = landmass;
        }

        // Functions
        public Land.Features.RoadNetwork generateRoads()
        {
            // 1. Find the top 1000 chunks by population
            List<Chunk> topChunks = new List<Chunk>();
            int lowestPopulation = int.MaxValue;
            Chunk lowestChunk = null;

            // a) Loop through all chunks to find the top 1000
            foreach (Chunk chunk in landmass.getChunks())
            {
                int chunkPopulation = chunk.getDemographics().getPopulation();
                
                // Only add and re-sort if we haven't got 1000 yet or this chunk is better than the worst in the list
                // TODO: There will be a more efficient way to do this
                if (topChunks.Count < 1000 || chunkPopulation > lowestPopulation)
                {
                    if (topChunks.Count >= 1000)
                    {
                        topChunks.Remove(lowestChunk);
                    }

                    topChunks.Add(chunk);

                    // Re-find the lowest chunk
                    lowestChunk = null;
                    lowestPopulation = int.MaxValue;
                    foreach (Chunk chunkTop in topChunks)
                    {
                        if (chunkTop.getDemographics().getPopulation() < lowestPopulation)
                        {
                            lowestChunk = chunkTop;
                            lowestPopulation = chunkTop.getDemographics().getPopulation();
                        }
                    }
                }
            }

            // b) Sort in descending order
            topChunks.Sort((obj1, obj2) => obj2.getDemographics().getPopulation().CompareTo(obj1.getDemographics().getPopulation()));

            // c) Create a hash set from the top 1000
            HashSet<Chunk> topChunksSet = new HashSet<Chunk>(topChunks);

            // 2. Iterate through these chunks and choose "nodes"
            List<Chunk> nodeChunks = new List<Chunk>();
            foreach (Chunk chunk in topChunks)
            {
                // Check if it is a surrounding chunk
                if (!topChunksSet.Contains(chunk))
                {
                    continue;
                }

                // a) Check for surrounding chunks to be excluded
                topChunksSet.Remove(chunk);
                for (int i = Mathf.Max(chunk.getRowIndex() - 1, 0); i <= Mathf.Min(chunk.getRowIndex() + 1, landmass.getChunkRows() - 1); i++)
                {
                    for (int j = Mathf.Max(chunk.getColIndex() - 1, 0); j <= Mathf.Min(chunk.getColIndex() + 1, landmass.getChunkCols() - 1); j++)
                    {
                        topChunksSet.Remove(landmass.getChunk(i, j));
                    }
                }

                // b) Add this to a list of nodes
                nodeChunks.Add(chunk);
            }

            // c) Sort in descending order
            nodeChunks.Sort((obj1, obj2) => obj2.getDemographics().getPopulation().CompareTo(obj1.getDemographics().getPopulation()));

            // 3. Connect the major X nodes(by population) together
            ChunkGraph chunkGraph = new ChunkGraph();
            for (int i = 0; i < 10; i++)
            {
                chunkGraph.addNode(nodeChunks[i]);

                // Link to the previous node
                // TODO: Implement a more realistic way of doing this
                if (i > 0)
                {
                    chunkGraph.linkNode(nodeChunks[i], nodeChunks[i - 1]);
                }
            }

            // 4. Iterate through connected nodes(only those connected can connect to others) until all nodes are connected
            while (chunkGraph.getNodes().Count < nodeChunks.Count)
            {
                /* Detail:
                 *  Iterate through connected nodes that haven't been iterated through yet
                 *      Connect to at least one unconnected node and zero or more connected nodes
                 *      (will prioritise those that are closest to it, maybe an element of population)
                 */
            }

            return null;
        }

        public Land.Landmass getLandmass()
        {
            return landmass;
        }
    }
}
