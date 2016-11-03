using Assets.Scripts.Land;
using Assets.Scripts.Land.Features;
using Assets.Scripts.Land.Features.Implementation;
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
        // Constants
        private const int NUM_MAJOR_CHUNKS = 1000;
        private const int MAXIMUM_CONNECTIONS = 2;

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
                if (topChunks.Count < NUM_MAJOR_CHUNKS || chunkPopulation > lowestPopulation)
                {
                    if (topChunks.Count >= NUM_MAJOR_CHUNKS)
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

            // a) Add all chunks as nodes
            foreach (Chunk chunk in nodeChunks)
            {
                chunkGraph.addNode(chunk);
            }

            // b) Connect major nodes together
            List<Chunk> majorNodes = new List<Chunk>();
            for (int i = 0; i < 10; i++)
            {
                majorNodes.Add(nodeChunks[i]);

                // Link to the previous node
                // TODO: Implement a more realistic way of doing this
                if (i > 0)
                {
                    chunkGraph.linkNode(nodeChunks[i], nodeChunks[i - 1]);
                }
            }

            // 4. Iterate through connected nodes(only those connected can connect to others) until all nodes are connected
            /* Detail:
                 *  Iterate through connected nodes that haven't been iterated through yet
                 *      Connect to at least one unconnected node and zero or more connected nodes
                 *      (will prioritise those that are closest to it, maybe an element of population)
                 */
            // TODO: There are disconnected pieces of the graph. This is due to circular links forming(ie. a node that creates
            //       a circular connection is closer than a graph far away that doesn't create a circular connection(kinda).
            //
            //       Maybe in better terms, there is a node further away that connects this node to more notes, not sure.
            //       
            //       Maybe deal with them at the end after everything else.
            HashSet<Chunk> unprocessedNodes = new HashSet<Chunk>(nodeChunks); // The nodes that have yet to be processed at all
            HashSet<Chunk> connectedGraphNodes = new HashSet<Chunk>(majorNodes); // These are the nodes that have been connected to the graph
            List<Chunk> nodesBeingProcessed = new List<Chunk>(majorNodes); // The nodes that are correctly being processed

            while (unprocessedNodes.Count > 0)
            {
                // Check if there are no nodes left waiting to be processed
                if (nodesBeingProcessed.Count == 0)
                {
                    // This means there are nodes that didn't get connected to the main graph
                    // Need to find the unprocessed node that is closest to the connected graph,
                    // connect that node and continue on processing from that.
                    Debug.Log("Leftover: " + unprocessedNodes.Count);

                    // Find the closest unprocessed node to the connected graph
                    Chunk closestUnprocessedNode = null;
                    Chunk closestConnectedNode = null;
                    float closestDistance = float.MaxValue;
                    foreach (Chunk unprocessedNode in unprocessedNodes)
                    {
                        foreach (Chunk connectedNode in connectedGraphNodes)
                        {
                            // Get the distance between the nodes
                            float distance = Mathf.Sqrt(Mathf.Pow(unprocessedNode.getRowIndex() - connectedNode.getRowIndex(), 2) + 
                                Mathf.Pow(unprocessedNode.getColIndex() - connectedNode.getColIndex(), 2));

                            // Check if it is closer than the top so far
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestUnprocessedNode = unprocessedNode;
                                closestConnectedNode = connectedNode;
                            }
                        }
                    }

                    // Connect these nodes together and process
                    // TODO: This is duplicated, best to refactor and remove later
                    chunkGraph.linkNode(closestConnectedNode, closestUnprocessedNode);
                    nodesBeingProcessed.Add(closestUnprocessedNode); // Node will now be processed
                    connectedGraphNodes.Add(closestUnprocessedNode); // Node is now connected to the graph

                    /*nodesBeingProcessed = new List<Chunk>(nodesToBeProcessed);
                    continue;*/
                }

                // These are nodes that have been connected and not processed
                HashSet<Chunk> nodesToBeProcessedNext = new HashSet<Chunk>(); // The nodes that will be processed next iteration
                int maximumConnections = MAXIMUM_CONNECTIONS;
                foreach (Chunk connectedNode in nodesBeingProcessed)
                {
                    List<Chunk> candidates = new List<Chunk>();

                    // a) Look for nodes in an outward expanding radius to connect to(a good way to find the closest nodes)
                    // The radius is a square radius with a width which starts at the range and expands outwards
                    int range = 1;
                    float rangeWidth = 1;
                    while (candidates.Count < maximumConnections) // Keep expanding until at least 2 candidates are found, 
                                                                  // one unlinked(unless no unlinked are left)
                    {
                        int rangeUpper = range + Mathf.FloorToInt(rangeWidth) - 1;
                        int rangeLower = range;

                        foreach (Chunk possibleConnection in nodeChunks)
                        {
                            // Check if the connection is within the desired radius(x/y +- range)
                            int rowRange = Mathf.Abs(possibleConnection.getRowIndex() - connectedNode.getRowIndex());
                            int colRange = Mathf.Abs(possibleConnection.getColIndex() - connectedNode.getColIndex());

                            if((rowRange >= rangeLower && rowRange <= rangeUpper && colRange <= rangeUpper) ||
                                (colRange >= rangeLower && colRange <= rangeUpper && rowRange <= rangeUpper))
                            {
                                // Make sure the node isn't linked to the candidate
                                if (!chunkGraph.isLinkedTo(connectedNode, possibleConnection)) 
                                {
                                    candidates.Add(possibleConnection);
                                }
                            }
                        }

                        // Increment the range/width higher as time goes on(ie. search more ground as there isn't anything close)
                        range += Mathf.FloorToInt(rangeWidth);
                        rangeWidth += 0.3f; // Some rate for the range increment
                    }

                    if (candidates.Count == 0)
                    {
                        Debug.Log("DEBUG: 0 candidates");
                    }

                    // b) Connect to a random selection of candidates
                    int totalCandidatesAllowed = UnityEngine.Random.Range(1, maximumConnections);
                    int totalCandidatesSoFar = 0;
                    while (totalCandidatesSoFar <= totalCandidatesAllowed && candidates.Count > 0)
                    {
                        // Choose a random candidate each time
                        Chunk candidate = candidates[UnityEngine.Random.Range(0, candidates.Count - 1)];

                        chunkGraph.linkNode(connectedNode, candidate);
                        nodesToBeProcessedNext.Add(candidate); // Node will be processed next
                        connectedGraphNodes.Add(candidate); // Node is now connected to the graph

                        totalCandidatesSoFar++;

                        // Make sure the candidate isn't selected again
                        candidates.Remove(candidate);
                    }

                    // c) Identify as processed and remove
                    unprocessedNodes.Remove(connectedNode);
                }

                // Remove existing nodes which are now processed, and add new nodes to process
                nodesBeingProcessed.Clear();
                foreach (Chunk chunk in nodesToBeProcessedNext)
                {
                    // Make sure it hasn't been processed yet
                    if (unprocessedNodes.Contains(chunk))
                    {
                        nodesBeingProcessed.Add(chunk);
                    }
                }

                nodesToBeProcessedNext.Clear();
            }

            // Last. Create a road network
            RoadNetwork roadNetwork = new RoadNetworkImpl();
            roadNetwork.setMainChunkConnections(chunkGraph);

            return roadNetwork;
        }

        public Land.Landmass getLandmass()
        {
            return landmass;
        }
    }
}
