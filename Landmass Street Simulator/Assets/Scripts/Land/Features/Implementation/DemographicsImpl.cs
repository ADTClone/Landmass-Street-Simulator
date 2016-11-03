using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Land.Features.Implementation
{
    class DemographicsImpl : Demographics
    {
        // Variables
        private Dictionary<Chunk, Structs.DemographicInfo> demographicsInfo;

        // Constructors
        public DemographicsImpl()
        {
            demographicsInfo = new Dictionary<Chunk, Structs.DemographicInfo>();
        }

        // Functions
        public Structs.DemographicInfo getDemographics(Chunk chunk)
        {
            return demographicsInfo[chunk];
        }

        public void setDemographics(Chunk chunk, Structs.DemographicInfo demographicsInfo)
        {
            this.demographicsInfo[chunk] = demographicsInfo;
            chunk.setDemographics(demographicsInfo);
        }
    }
}
