using Assets.Scripts.Land.Features.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Land
{
    /* A chunk in terms of area, related to a landmass.
     * 
     * A chunk has a position, as well as a size. It will also
     * have references to information related about it.
     */
    public class Chunk
    {
        // Variables
        private int rowIndex;
        private int colIndex;
        private DemographicInfo demographics;

        // Constructors
        public Chunk(int rowIndex, int colIndex)
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;

            this.demographics = null;
        }

        // Functions
        public int getRowIndex()
        {
            return rowIndex;
        }

        public int getColIndex()
        {
            return colIndex;
        }

        public void setDemographics(DemographicInfo demographics)
        {
            this.demographics = demographics;
        }

        public DemographicInfo getDemographics()
        {
            return demographics;
        }
    }
}
