using Assets.Scripts.Land;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    /// A graph specifically for chunks.
    /// 
    /// Note: Should probably find an existing standard. This will do for
    /// now though.
    /// </summary>
    public class ChunkGraph
    {
        // Variables
        private HashSet<Chunk> nodes;
        private Dictionary<Chunk, HashSet<Chunk>> links;

        // Constructors
        public ChunkGraph()
        {
            this.nodes = new HashSet<Chunk>();
            this.links = new Dictionary<Chunk, HashSet<Chunk>>();
        }

        // Functions
        /// <summary>
        /// Adds a node to the graph(with no connections).
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void addNode(Chunk node)
        {
            if (!nodes.Contains(node))
            {
                nodes.Add(node);
                links.Add(node, new HashSet<Chunk>());
            }
        }

        /// <summary>
        /// Links a node in the graph to another node.
        /// 
        /// Both nodes must have been added before they can be linked.
        /// </summary>
        /// <param name="nodeOne">The first node to link.</param>
        /// <param name="nodeTwo">The second node to link.</param>
        public void linkNode(Chunk nodeOne, Chunk nodeTwo)
        {
            links[nodeOne].Add(nodeTwo);
            links[nodeTwo].Add(nodeOne);
        }

        /// <summary>
        /// Gets all nodes connected to the given node.
        /// </summary>
        /// <param name="node">The node to get the links for.</param>
        /// <returns>A unique set of nodes linked to the given node.</returns>
        public HashSet<Chunk> getConnectedNodes(Chunk node)
        {
            return links[node];
        }

        /// <summary>
        /// Gets all nodes in the graph.
        /// </summary>
        /// <returns>All nodes in the graph.</returns>
        public HashSet<Chunk> getNodes()
        {
            return nodes;
        }

        /// <summary>
        /// Checks whether the node has been linked in the graph.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if it links to at least one other node, false otherwise.</returns>
        public bool isLinked(Chunk node)
        {
            return links.ContainsKey(node) && getConnectedNodes(node).Count > 0;
        }

        /// <summary>
        /// Checks whether the node is linked to another specified node.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <param name="linkedTo">The node to check whether it is linked to.</param>
        /// <returns>True if the node is linked to the other node.</returns>
        public bool isLinkedTo(Chunk node, Chunk linkedTo)
        {
            return links[node].Contains(linkedTo);
        }
    }
}
