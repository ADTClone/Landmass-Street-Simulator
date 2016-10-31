using Assets.Scripts.Land.Features;
using System.Collections.Generic;

namespace Assets.Scripts.Land
{
    /* Represents a landmass.
     * 
     * A landmass is a segment of land with the following properties:
     *  - has a set dimension
     *  - broken into accessible chunks
     *  - has geographical information(elevation, rivers, terrain etc)
     *  - has population information(counts, social etc)
     *  - has road network information
     *  - has housing information
     */
    public class Landmass
    {
        // Variables
        private float length;
        private float width;
        private float chunkSize;
        private int chunkRows;
        private int chunkCols;
        private Terrain terrain;
        private Demographics demographics;
        private RoadNetwork roadNetwork;
        private Housing housing;
        private Chunk[,] chunks; // First index is row, second index is column
        private List<Chunk> allChunks; 

        // Constructors
        public Landmass()
        {
            this.length = 0;
            this.width = 0;
            this.chunkSize = 0;
            this.chunkRows = 0;
            this.chunkCols = 0;
            this.chunks = new Chunk[0, 0];
            this.allChunks = new List<Chunk>();
        }

        public Landmass(float length, float width, float chunkSize) : this()
        {
            this.length = length;
            this.width = width;
            this.chunkSize = chunkSize;

            setupChunks(length, width, chunkSize);
        }

        // Functions
        /* Sets up the chunks for use in other systems.
         * 
         * @param length The length of the landmass.
         * @param width The width of the landmass.
         * @param chunkSize The size of each chunk.
         */
        private void setupChunks(float length, float width, float chunkSize)
        {
            // Determine the chunk rows and columns
            chunkRows = UnityEngine.Mathf.CeilToInt(length / chunkSize);
            chunkCols = UnityEngine.Mathf.CeilToInt(width / chunkSize);

            // Create the chunks
            chunks = new Chunk[chunkRows, chunkCols];
            for (int rows = 0; rows < chunkRows; rows++)
            {
                for (int cols = 0; cols < chunkCols; cols++)
                {
                    Chunk newChunk = new Chunk(rows, cols);
                    chunks[rows, cols] = newChunk;
                    allChunks.Add(newChunk);
                }
            }
        }

        public Chunk getChunk(int row, int col)
        {
            return chunks[row, col];
        }

        public List<Chunk> getChunks()
        {
            return allChunks;
        }

        public void setTerrain(Terrain terrain)
        {
            this.terrain = terrain;
        }

        public void setDemographics(Demographics demographics)
        {
            this.demographics = demographics;
        }

        public void setRoadNetwork(RoadNetwork roadNetwork)
        {
            this.roadNetwork = roadNetwork;
        }

        public void setHousing(Housing housing)
        {
            this.housing = housing;
        }

        public float getLength()
        {
            return length;
        }

        public float getWidth()
        {
            return width;
        }

        public float getChunkSize()
        {
            return chunkSize;
        }

        public int getChunkRows()
        {
            return chunkRows;
        }

        public int getChunkCols()
        {
            return chunkCols;
        }

        public Terrain getTerrain()
        {
            return terrain;
        }

        public Demographics getDemographics()
        {
            return demographics;
        }

        public RoadNetwork getRoadNetwork()
        {
            return roadNetwork;
        }

        public Housing getHousing()
        {
            return housing;
        }
    }
}
