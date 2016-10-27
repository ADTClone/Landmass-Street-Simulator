using Assets.Scripts.Land.Features;

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
        private Terrain terrain;
        private Demographics demographics;
        private RoadNetwork roadNetwork;
        private Housing housing;

        // Constructors
        public Landmass()
        {
            this.length = 0;
            this.width = 0;
            this.chunkSize = 0;
        }

        public Landmass(float length, float width, float chunkSize) : base()
        {
            this.length = length;
            this.width = width;
            this.chunkSize = chunkSize;
        }

        // Functions
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
