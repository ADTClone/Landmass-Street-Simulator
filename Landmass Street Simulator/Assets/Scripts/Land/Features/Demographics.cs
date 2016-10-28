using Assets.Scripts.Land.Features.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Land.Features
{
    public interface Demographics
    {
        /* Gets the demographic information for a chunk */
        DemographicInfo getDemographics(Chunk chunk);

        /* Sets the demographics information for a chunk */
        void setDemographics(Chunk chunk, DemographicInfo demographicsInfo);
    }
}
