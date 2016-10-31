using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Land;
using Assets.Scripts.Generator;
using Assets.Scripts.Generator.Prototype;
using Assets.Scripts.Land.Features;
using Assets.Scripts.Land.Features.Structs;
using Assets.Scripts.Generator.Implementation;

public class TestHarness : MonoBehaviour {
    // Variables
    private Landmass landmass;

	// Functions
	void Start () {
        runHarness();
	}

    void Update()
    {
        // Simple debug for sorting
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Sort by population in demographics desc
            List<Chunk> chunks = new List<Chunk>(landmass.getChunks());
            chunks.Sort((obj1, obj2) => obj2.getDemographics().getPopulation().CompareTo(obj1.getDemographics().getPopulation()));

            // List top 10
            string top10 = "";
            for (int i = 0; i < Mathf.Min(chunks.Count, 10); i++)
            {
                top10 += chunks[i].getDemographics().getPopulation() + " ";
            }
            Debug.Log(top10);
        }
    }

    void OnGUI()
    {
        float totalHeight = 300;
        float totalWidth = 300;
        float startX = 100;
        float startY = 100;

        // Display demographics
        if (Input.GetKey(KeyCode.D))
        {
            for (int row = 0; row < landmass.getChunkRows(); row++)
            {
                for (int col = 0; col < landmass.getChunkCols(); col++)
                {
                    Chunk chunk = landmass.getChunk(row, col);
                    DemographicInfo info = landmass.getDemographics().getDemographics(chunk);

                    GUI.Box(new Rect(startX + row * (totalWidth / landmass.getChunkRows()), 
                        startY + col * (totalHeight / landmass.getChunkCols()),
                        totalWidth / landmass.getChunkRows(), 
                        totalHeight / landmass.getChunkCols()), 
                        info.getPopulation().ToString());
                }
            }
        }
    }

    private void runHarness()
    {
        // 1. Create a landmass
        landmass = new Landmass(100000, 100000, 1000);

        Debug.Log("Number of chunks: " + landmass.getChunks().Count);

        // 2. Generate the terrain for the landmass
        TerrainGenerator terrainGenerator = new StubTerrainGenerator(landmass);
        Assets.Scripts.Land.Features.Terrain terrain = terrainGenerator.generateTerrain();
        landmass.setTerrain(terrain);

        Debug.Log("Terrain of first chunk = " + terrain.getTerrainType(landmass.getChunks()[0]));

        // 3. Generate the population for the landmass
        PopulationGenerator populationGenerator = new StubPopulationGenerator(landmass);
        Demographics demographics = populationGenerator.generatePopulation();
        landmass.setDemographics(demographics);

        Debug.Log("Population of first chunk = " + demographics.getDemographics(landmass.getChunks()[0]).getPopulation());

        // 4. Generate the road network for the landmass
        RoadGenerator roadGenerator = new RoadGeneratorImpl(landmass);
        roadGenerator.generateRoads();
    }
}
