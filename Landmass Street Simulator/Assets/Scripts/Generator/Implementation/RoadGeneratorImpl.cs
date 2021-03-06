﻿using Assets.Scripts.Land;
using Assets.Scripts.Land.Features;
using Assets.Scripts.Land.Features.Implementation;
using Assets.Scripts.Land.Features.Structs;
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
            // 1. Get a list of all of the cities and suburbs, adding their chunks as nodes
            List<Chunk> nodeChunks = new List<Chunk>();
            List<Chunk> majorNodes = new List<Chunk>();

            // a) Loop through all cities and their suburbs, and add the nodes
            foreach (City city in landmass.getDemographics().getCities())
            {
                // Adds the city chunk to the major nodes
                majorNodes.Add(city.getCenter().getCenter());

                foreach (Suburb suburb in landmass.getDemographics().getSuburbs())
                {
                    // Adds the suburb center chunk to the nodes
                    nodeChunks.Add(suburb.getCenter());
                }
            }

            // c) Sort in descending order
            nodeChunks.Sort((obj1, obj2) => obj2.getDemographics().getPopulation().CompareTo(obj1.getDemographics().getPopulation()));

            // 2. Connect the major X nodes(by population) together
            ChunkGraph chunkGraph = new ChunkGraph();

            // a) Add all chunks as nodes
            foreach (Chunk chunk in nodeChunks)
            {
                chunkGraph.addNode(chunk);
            }

            // b) Connect major nodes together
            for (int i = 0; i < majorNodes.Count; i++)
            {
                // Link to the previous node
                // TODO: Implement a more realistic way of doing this
                if (i > 0)
                {
                    chunkGraph.linkNode(majorNodes[i], majorNodes[i - 1]);
                }
            }

            // 3. Iterate through connected nodes(only those connected can connect to others) until all nodes are connected
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
            HashSet<Chunk> unconnectedGraphNodes = new HashSet<Chunk>();
            List<Chunk> nodesBeingProcessed = new List<Chunk>(majorNodes); // The nodes that are correctly being processed
            bool addToConnectedGraph = true;

            while (unprocessedNodes.Count > 0)
            {
                // Check if there are no nodes left waiting to be processed
                if (nodesBeingProcessed.Count == 0)
                {
                    // Add all leftover nodes to the to process list
                    nodesBeingProcessed = new List<Chunk>(unprocessedNodes);

                    // Add the left over to the unconnected graph nodes, as they will not be connected
                    unconnectedGraphNodes = new HashSet<Chunk>(unprocessedNodes);

                    addToConnectedGraph = false; // None of these will be "connected"
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

                        if (addToConnectedGraph) // Only do this if we are to add it to the connected graph
                        {
                            nodesToBeProcessedNext.Add(candidate); // Node will be processed next
                            connectedGraphNodes.Add(candidate); // Node is now connected to the graph
                        }

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

            // Everything is processed, now just need to deal with the nodes not confirmed to be connected
            Debug.Log("Leftover(unconnected) " + unconnectedGraphNodes.Count);

            // Precalculcate the closest distance
            Chunk closestUnconnectedNode = null;
            Chunk closestConnectedNode = null;
            float closestDistance = float.MaxValue;
            Dictionary<Chunk, float> unconnectedClosestDistance = new Dictionary<Chunk, float>();
            Dictionary<Chunk, Chunk> unconnectedClosestChunk = new Dictionary<Chunk, Chunk>();

            foreach (Chunk unconnectedNode in unconnectedGraphNodes)
            {
                Chunk localClosestConnectedNode = null;
                float localClosestDistance = float.MaxValue;

                foreach (Chunk connectedNode in connectedGraphNodes)
                {
                    // Get the distance between the nodes
                    float distance = Mathf.Sqrt(Mathf.Pow(unconnectedNode.getRowIndex() - connectedNode.getRowIndex(), 2) +
                        Mathf.Pow(unconnectedNode.getColIndex() - connectedNode.getColIndex(), 2));

                    // Check if it is closer than the top so far for this unconnected chunk
                    if (distance < localClosestDistance)
                    {
                        localClosestDistance = distance;
                        localClosestConnectedNode = connectedNode;
                    }
                }

                // Add to the dictionary for lookup later
                unconnectedClosestDistance[unconnectedNode] = localClosestDistance;
                unconnectedClosestChunk[unconnectedNode] = localClosestConnectedNode;

                // Check globally
                if (localClosestDistance < closestDistance)
                {
                    closestDistance = localClosestDistance;
                    closestUnconnectedNode = unconnectedNode;
                    closestConnectedNode = localClosestConnectedNode;
                }
            }

            // Find the closest unprocessed node to the connected graph
            while (unconnectedGraphNodes.Count > 0)
            {
                Debug.Log("Leftover: " + unconnectedGraphNodes.Count);

                // Connect these nodes together and process
                // TODO: This is duplicated, best to refactor and remove later
                chunkGraph.linkNode(closestConnectedNode, closestUnconnectedNode);

                connectedGraphNodes.Add(closestUnconnectedNode); // Node is now connected to the graph
                unconnectedGraphNodes.Remove(closestUnconnectedNode);
                foreach (Chunk unconnectedNode in unconnectedGraphNodes) // Check if this newly connected node is closer to any of the other nodes
                {
                    // Get the distance between the nodes
                    float distance = Mathf.Sqrt(Mathf.Pow(unconnectedNode.getRowIndex() - closestUnconnectedNode.getRowIndex(), 2) +
                        Mathf.Pow(unconnectedNode.getColIndex() - closestUnconnectedNode.getColIndex(), 2));

                    if (distance < unconnectedClosestDistance[unconnectedNode]) // Check if it is locally better
                    {
                        unconnectedClosestDistance[unconnectedNode] = distance;
                        unconnectedClosestChunk[unconnectedNode] = closestUnconnectedNode;
                    }
                }

                // Recursively find all nodes connected to it and mark them as connected too
                HashSet<Chunk> newlyConnectedNodes = findOtherConnectedNodes(chunkGraph, unconnectedGraphNodes, closestUnconnectedNode);
                foreach (Chunk newlyConnectedNode in newlyConnectedNodes)
                {
                    connectedGraphNodes.Add(newlyConnectedNode); // Node is now connected to the graph
                    unconnectedGraphNodes.Remove(newlyConnectedNode);

                    foreach (Chunk unconnectedNode in unconnectedGraphNodes) // Check if this newly connected node is closer to any of the other nodes
                    {
                        // Get the distance between the nodes
                        float distance = Mathf.Sqrt(Mathf.Pow(unconnectedNode.getRowIndex() - newlyConnectedNode.getRowIndex(), 2) +
                            Mathf.Pow(unconnectedNode.getColIndex() - newlyConnectedNode.getColIndex(), 2));

                        if (distance < unconnectedClosestDistance[unconnectedNode]) // Check if it is locally better
                        {
                            unconnectedClosestDistance[unconnectedNode] = distance;
                            unconnectedClosestChunk[unconnectedNode] = newlyConnectedNode;
                        }
                    }
                }

                // Work out the new closest node
                closestDistance = float.MaxValue;
                foreach (Chunk unconnectedNode in unconnectedGraphNodes)
                {
                    float nodeDistance = unconnectedClosestDistance[unconnectedNode];
                    if (nodeDistance < closestDistance)
                    {
                        closestDistance = nodeDistance;
                        closestUnconnectedNode = unconnectedNode;
                        closestConnectedNode = unconnectedClosestChunk[unconnectedNode];
                    }
                }
            }

            // Last. Create a road network
            RoadNetwork roadNetwork = new RoadNetworkImpl();
            roadNetwork.setMainChunkConnections(chunkGraph);

            return roadNetwork;
        }

        private HashSet<Chunk> findOtherConnectedNodes(ChunkGraph chunkGraph, HashSet<Chunk> unconnectedNodes, Chunk node)
        {
            HashSet<Chunk> newlyConnectedNodes = new HashSet<Chunk>();

            findOtherConnectedNodes(chunkGraph, unconnectedNodes, node, newlyConnectedNodes);

            return newlyConnectedNodes;
        }

        private void findOtherConnectedNodes(ChunkGraph chunkGraph, HashSet<Chunk> unconnectedNodes, Chunk node, HashSet<Chunk> newlyConnectedNodes)
        {
            foreach (Chunk linkedChunk in chunkGraph.getConnectedNodes(node))
            {
                // Check it is unconnected and hasn't already been found
                if (unconnectedNodes.Contains(linkedChunk) && !newlyConnectedNodes.Contains(linkedChunk))
                {
                    // It is now connected
                    newlyConnectedNodes.Add(linkedChunk);

                    findOtherConnectedNodes(chunkGraph, unconnectedNodes, linkedChunk, newlyConnectedNodes);
                }
            }
        }

        public Land.Landmass getLandmass()
        {
            return landmass;
        }
    }
}
