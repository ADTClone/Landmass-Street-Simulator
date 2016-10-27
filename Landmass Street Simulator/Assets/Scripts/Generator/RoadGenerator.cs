
using Assets.Scripts.Land;
using Assets.Scripts.Land.Features;

/* Generates roads based off demographical and other information for
 * a landmass.
 * 
 * Before generation, information about the landmass will be given.
 * 
 * After the generation, a network of roads will have been created for
 * the landmass.
 */
namespace Assets.Scripts.Generator
{
    public interface RoadGenerator
    {
        /* Generates an entire road network for the associated
         * landmass.
         * 
         * @returns The road network for the landmass.
         */
        RoadNetwork generateRoads();

        /* Gets the land mass associated to the generator.
         * 
         * @returns Landmass The associated landmass.
         */
        Landmass getLandmass();
    }
}
