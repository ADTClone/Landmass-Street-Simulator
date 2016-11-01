using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Land.Features
{
    public interface RoadNetwork
    {
        /// <summary>
        /// A possibly temporary function to get the high level chunk
        /// connections in terms of a graph.
        /// </summary>
        /// <returns>High level chunk connections</returns>
        ChunkGraph getMainChunkConnections();

        /// <summary>
        /// Sets the high level chunk connections
        /// </summary>
        void setMainChunkConnections(ChunkGraph chunkGraph);
    }
}
