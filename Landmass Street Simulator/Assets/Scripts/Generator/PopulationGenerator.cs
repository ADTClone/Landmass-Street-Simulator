using Assets.Scripts.Land;
using Assets.Scripts.Land.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Generator
{
    public interface PopulationGenerator
    {
        Demographics generatePopulation();
        Landmass getLandmass();
    }
}
