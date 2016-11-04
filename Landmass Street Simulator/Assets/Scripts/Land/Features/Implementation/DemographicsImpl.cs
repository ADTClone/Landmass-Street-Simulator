using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Land.Features.Structs;

namespace Assets.Scripts.Land.Features.Implementation
{
    class DemographicsImpl : Demographics
    { 
        // Variables
        private Dictionary<Chunk, DemographicInfo> demographicsInfo;
        private List<City> cities;
        private List<Suburb> suburbs;
        private HashSet<Chunk> cityChunks;
        private HashSet<Chunk> suburbChunks;

        // Constructors
        public DemographicsImpl()
        {
            demographicsInfo = new Dictionary<Chunk, DemographicInfo>();
            cities = new List<City>();
            suburbs = new List<Suburb>();
            cityChunks = new HashSet<Chunk>();
            suburbChunks = new HashSet<Chunk>();
        }

        // Functions
        public DemographicInfo getDemographics(Chunk chunk)
        {
            return demographicsInfo[chunk];
        }

        public List<City> getCities()
        {
            return cities;
        }

        public List<Suburb> getSuburbs()
        {
            return suburbs;
        }

        public void setDemographics(Chunk chunk, DemographicInfo demographicsInfo)
        {
            this.demographicsInfo[chunk] = demographicsInfo;
            chunk.setDemographics(demographicsInfo);
        }

        public void addCity(City city)
        {
            cities.Add(city);
            cityChunks.Add(city.getCenter().getCenter());
        }

        public void addSuburb(Suburb suburb)
        {
            suburbs.Add(suburb);
            suburbChunks.Add(suburb.getCenter());
        }


        public bool isCity(Chunk chunk)
        {
            return cityChunks.Contains(chunk);
        }

        public bool isSuburb(Chunk chunk)
        {
            return suburbChunks.Contains(chunk);
        }
    }
}
