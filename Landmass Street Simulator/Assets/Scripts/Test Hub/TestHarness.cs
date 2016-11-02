using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Land;
using Assets.Scripts.Generator;
using Assets.Scripts.Generator.Prototype;
using Assets.Scripts.Land.Features;
using Assets.Scripts.Land.Features.Structs;
using Assets.Scripts.Generator.Implementation;
using Assets.Scripts.Utilities;

public class TestHarness : MonoBehaviour {
    // Variables
    public GameObject nodePrefab;

    private Landmass landmass;
    private GameObject roadRepresentation;
    private List<GameObject> roadRepresentationObjects;

	// Functions
	void Start () {
        roadRepresentation = null;
        roadRepresentationObjects = new List<GameObject>();

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
        // Graphical display of road network
        if (Input.GetKeyDown(KeyCode.W))
        {
            displayRoadRepresentation();
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
        landmass = new Landmass(1000000, 1000000, 1000);

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
        RoadNetwork roadNetwork = roadGenerator.generateRoads();
        landmass.setRoadNetwork(roadNetwork);
    }

    /// <summary>
    /// A simple testing function to graph the road network
    /// </summary>
    private void displayRoadRepresentation()
    {
        if (roadRepresentation != null)
        {
            foreach (GameObject obj in roadRepresentationObjects)
            {
                Destroy(obj);
            }

            Destroy(roadRepresentation);

            roadRepresentationObjects.Clear();
        }

        roadRepresentation = new GameObject();
        roadRepresentation.transform.position = new Vector2(0, 0);

        // Setup graph
        ChunkGraph graph = landmass.getRoadNetwork().getMainChunkConnections();
        HashSet<Chunk> drawnChunks = new HashSet<Chunk>();

        // Determine dimensions
        float rowDim = 10;
        float colDim = 10;
        float rowsFactor = rowDim / landmass.getChunkRows();
        float colsFactor = colDim / landmass.getChunkCols();

        // Draw the graph
        foreach(Chunk chunk in graph.getNodes()) {
            // Create the lines for connections
            foreach (Chunk connectedChunk in graph.getConnectedNodes(chunk))
            {
                if (!drawnChunks.Contains(connectedChunk))
                {
                    // Create line
                    GameObject line = createLine(new Vector2(chunk.getRowIndex() * rowsFactor - rowDim / 2.0f,
                        chunk.getColIndex() * colsFactor - colDim / 2.0f),
                        new Vector2(connectedChunk.getRowIndex() * rowsFactor - rowDim / 2.0f,
                            connectedChunk.getColIndex() * colsFactor - colDim / 2.0f),
                        Color.red);

                    roadRepresentationObjects.Add(line);
                }
            }

            // Create the node
            GameObject node = createNode(new Vector2(chunk.getRowIndex() * rowsFactor - rowDim / 2.0f,
                        chunk.getColIndex() * colsFactor - colDim / 2.0f));
            roadRepresentationObjects.Add(node);

            drawnChunks.Add(chunk);
        }
    }

    /// <summary>
    /// Creates a line between the two points, and returns a game object representing
    /// this line.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private GameObject createLine(Vector2 from, Vector2 to, Color colour)
    {
        GameObject gameObject = new GameObject();
        LineRenderer line = gameObject.AddComponent<LineRenderer>();

        line.SetPositions(new Vector3[] { new Vector3(from.x, from.y, 0), new Vector3(to.x, to.y, 0)});
        line.SetWidth(0.03f, 0.03f);
        line.SetColors(colour, colour);

        return gameObject;
    }

    private GameObject createNode(Vector2 position)
    {
        GameObject node = Instantiate(nodePrefab);
        node.transform.position = new Vector3(position.x, position.y);

        return node;
    }
}
