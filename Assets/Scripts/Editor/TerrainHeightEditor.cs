using UnityEngine;
using UnityEditor;
using System.Collections;

/*
    -> Adjust Terrain
    -> Change Resolution
    -> Change Size
    -> Copy from other Terrain
    -> Fit Resolution to Size

*/

public class TerrainHeightEditor : EditorWindow
{
    private static Terrain activeTerrain;
    private static TerrainData activeTerrainData;

    private static Terrain referencedTerrain;
    private static TerrainData referencedTerrainData;

    private bool copyTextures;
    private static Transform heightTransform;

    // private float goodResolution = /*~*/1;
    
    private float heightAdjustment;
    private float[,] heightBackup;

    // change size
    private int newWidth;
    private int newLength;
    private int baseHeight;
    private bool fitResolution;

    private static bool setupReferences = true;
    
    [MenuItem("Terrain/HeightEditor")]
    static void Init()
    {
        TerrainHeightEditor window = GetWindow<TerrainHeightEditor>(typeof(TerrainHeightEditor));
        window.Show();

        setupReferences = true;
    }

    void OnGUI()
    {
        if (setupReferences || !activeTerrain)
        {
            setupReferences = true;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            SetupReferences();
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Start Editing"))
            {
                setupReferences = false;
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditTerrain();
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Settings"))
            {
                setupReferences = true;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    void SetupReferences()
    {
        if (activeTerrain = EditorGUILayout.ObjectField("Active Terrain: ", activeTerrain, typeof(Terrain), true) as Terrain)
        {
            activeTerrainData = activeTerrain.terrainData;
        }

        if (referencedTerrain = EditorGUILayout.ObjectField("Referenced Terrain: ", referencedTerrain, typeof(Terrain), true) as Terrain)
        {
            referencedTerrainData = referencedTerrain.terrainData;
        }

        heightTransform = EditorGUILayout.ObjectField("Referenced Transform: ", heightTransform, typeof(Transform), true) as Transform;
    }

    void EditTerrain()
    {
        // Adjust Terrain
        GUILayout.Label("Adjust Terrain:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        heightAdjustment = EditorGUILayout.Slider("Height Adjustment: ", heightAdjustment, -1, 1);
        
        if (GUILayout.Button("Adjust Terrain"))
        {
            AdjustTerrain();
        }
        
        EditorGUILayout.EndHorizontal();

        // Change Size
        GUILayout.Label("Change Size:", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        GUILayout.Label("Current Size:");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Width:", activeTerrainData.size.x);
        EditorGUILayout.FloatField("Length:", activeTerrainData.size.z);
        EditorGUILayout.EndHorizontal();
        float currentDensity = (activeTerrainData.heightmapResolution * activeTerrainData.heightmapResolution) / (activeTerrainData.size.x * activeTerrainData.size.z);
        GUILayout.Label("Current Density: " + currentDensity);

        GUILayout.Label("New Size:");

        EditorGUILayout.BeginHorizontal();
        newWidth = EditorGUILayout.IntField("Width:", newWidth);
        newLength = EditorGUILayout.IntField("Length:", newLength);
        EditorGUILayout.EndHorizontal();
        // baseHeight = EditorGUILayout.IntField("BaseHeight:", baseHeight);

        float newDensity = 0;
        if (newWidth * newLength > 0)
        {
            newDensity = (activeTerrainData.heightmapResolution * activeTerrainData.heightmapResolution) / (float)(newWidth * newLength);
        }
        GUILayout.Label("New Density: " + newDensity);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Update Size"))
        {
            UpdateSize();
        }
        fitResolution = EditorGUILayout.Toggle("FitResolution", fitResolution);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        // copy heightMap
        GUILayout.Label("Copy Heightmap from other Terrain:", EditorStyles.boldLabel);
        if(referencedTerrain)
        {
            copyTextures = EditorGUILayout.Toggle("CopyTextures", copyTextures);
            if (GUILayout.Button("Transfer Heightmap"))
            {
                TransferHeightmap();
            }
        }
        else
        {
            GUILayout.Label("referenced terrain missing");
        }

        // get height at position
        if (heightTransform)
        {
            GUILayout.Label("Height: " + activeTerrain.SampleHeight(heightTransform.position));
        }
    }
    
    void AdjustTerrain()
    {
        // get old height matrix
        float[,] heights = activeTerrainData.GetHeights(0, 0, activeTerrainData.heightmapWidth, activeTerrainData.heightmapHeight);
        heightBackup = heights;

        // move every point up/down
        for (int y = 0; y < activeTerrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < activeTerrainData.heightmapWidth; x++)
            {
                heights[y, x] = heights[y, x] + heightAdjustment;
            }
        }

        // set new heights to terrain
        activeTerrainData.SetHeights(0, 0, heights);
        
        // calc new worldHeight
        float offset = -heightAdjustment * activeTerrainData.size.y;                    // heightAdjustment > 0 => terrain moves up
        Vector3 currentPosition = activeTerrain.transform.position;                     // => to relativize move pivot down
        Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y + offset, currentPosition.z);

        // set new worldHeight
        activeTerrain.transform.position = newPosition;
    }

    void UpdateSize()
    {
        // get current variables
        // float curWidth = activeTerrainData.size.x;
        // float curLength = activeTerrainData.size.z;
        // int curResolution = activeTerrainData.heightmapResolution;
        heightBackup = activeTerrainData.GetHeights(0, 0, activeTerrainData.heightmapWidth, activeTerrainData.heightmapHeight);
        
        // add current terrain
        float[,] heights = new float[newWidth, newLength];

        for (int y = 0; y < Mathf.Min(activeTerrainData.heightmapHeight, newWidth); y++)
        {
            for (int x = 0; x < Mathf.Min(activeTerrainData.heightmapWidth, newLength); x++)
            {
                heights[y, x] = heightBackup[y, x];
            }
        }
        /*
        // add additional heights for new positions
        for (int y = activeTerrainData.heightmapHeight; y < newWidth; y++)
        {
            for (int x = activeTerrainData.heightmapWidth; x < newLength; x++)
            {
                heights[y, x] = baseHeight;
            }
        }
        */

        // fit resolution?
        if (fitResolution)
        {
            FitResolution();
        }

        // set new size
        activeTerrainData.size = new Vector3(newWidth, activeTerrainData.size.y, newLength);

        // set new heights
        activeTerrainData.SetHeights(0, 0, heights);

        // set new textures
    }

    void FitResolution()
    {
        float curDensity = (activeTerrainData.heightmapResolution * activeTerrainData.heightmapResolution) / (activeTerrainData.size.x * activeTerrainData.size.z);
        int newDensity = (int)Mathf.Sqrt(curDensity * (newWidth * newLength));

        activeTerrainData.alphamapResolution = newDensity;
        activeTerrainData.baseMapResolution = newDensity;
        // activeTerrainData.detailResolution = newDensity;
        activeTerrainData.heightmapResolution = newDensity;
    }

    void TransferHeightmap()
    {
        // get heightDifference
        float heightDifference = (referencedTerrain.GetPosition().y - activeTerrain.GetPosition().y) / activeTerrainData.size.y; // referencedTerrainData.size.y - activeTerrainData.size.y;

        // get overlapping part
        float minX = Mathf.Max(activeTerrain.GetPosition().x, referencedTerrain.GetPosition().x);
        float maxX = Mathf.Min(activeTerrain.GetPosition().x + activeTerrainData.size.x, referencedTerrain.GetPosition().x + referencedTerrainData.size.x);
        float minZ = Mathf.Max(activeTerrain.GetPosition().z, referencedTerrain.GetPosition().z);
        float maxZ = Mathf.Min(activeTerrain.GetPosition().z + activeTerrainData.size.z, referencedTerrain.GetPosition().z + referencedTerrainData.size.z);
        
        Vector3 worldspaceMinPosition = new Vector3(minX, 0, minZ);
        Vector3 worldspaceMaxPosition = new Vector3(maxX, 0, maxZ);

        // Debug.Log("worldspaceMinPosition: " + worldspaceMinPosition + " worldspaceMaxPosition: " + worldspaceMaxPosition);

        // transform position to active terrainSpace
        Vector3 relativeMinPosition = worldspaceMinPosition - activeTerrain.GetPosition();
        Vector3 relativeMaxPosition = worldspaceMaxPosition - activeTerrain.GetPosition();

        float xDensity = activeTerrainData.size.x / activeTerrainData.heightmapWidth;
        float zDensity = activeTerrainData.size.z / activeTerrainData.heightmapHeight;

        // Debug.Log(activeTerrainData.size + ", " + activeTerrainData.heightmapWidth + ", " + activeTerrainData.heightmapHeight + ", " + xDensity + ", " + zDensity);

        // get samples at terrain
        int xBase = Mathf.RoundToInt(relativeMinPosition.x / xDensity);
        int zBase = Mathf.RoundToInt(relativeMinPosition.z / zDensity);

        int xLast = Mathf.RoundToInt(relativeMaxPosition.x / xDensity);
        int zLast = Mathf.RoundToInt(relativeMaxPosition.z / zDensity);

        // get heightmap from referenced terrain
        float[,] referencedHeights = new float[zLast - zBase, xLast - xBase];
        // float[,,] referendedAlphas = new float[zLast - zBase, xLast - xBase];
        
        for (int z = zBase; z < zLast; z++)
        {
            // get worldZ of current sample
            float worldPosZ = worldspaceMinPosition.z + (z - zBase) * zDensity;
            
            for (int x = xBase; x < xLast; x++)
            {
                // get worldX of current sample
                float worldPosX = worldspaceMinPosition.x + (x - xBase) * xDensity;

                // get new height
                referencedHeights[z, x] = referencedTerrain.SampleHeight(new Vector3(worldPosX, 0, worldPosZ)) / activeTerrainData.size.y;

                // compensate different root heights
                referencedHeights[z, x] += heightDifference;
            }
        }
        // Debug.Log(referencedHeights[0, 0] + ", " + referencedHeights[0, xLast - 1] + ", " + referencedHeights[zLast - 1, 0] + ", " + referencedHeights[zLast - 1, xLast - 1]);

        if(heightTransform)
        {
            Vector3 testPosition = new Vector3(worldspaceMinPosition.x + (xLast - xBase) * xDensity, 0, worldspaceMinPosition.z + (zLast - zBase) * zDensity);
            heightTransform.position = testPosition;
            // Debug.Log(worldspaceMinPosition.x + ", " + xLast + ", " + xBase);
            // Debug.Log(worldspaceMinPosition.z + ", " + zLast + ", " + zBase);
        }

        // transfer heightmap to activeTerrain
        activeTerrainData.SetHeights(zBase, xBase, referencedHeights);
        // activeTerrainData.SetAlphamaps();
    }

    private float[] GetTextureMix(Vector3 WorldPos, Terrain terrain)
    {
        Vector3 terrainPos = terrain.GetPosition();
        TerrainData terrainData = terrain.terrainData;
        // returns an array containing the relative mix of textures
        // on the main terrain at this world position.

        // The number of values in the array will equal the number
        // of textures added to the terrain.

        // calculate which splat map cell the worldPos falls within (ignoring y)
        int mapX = (int)(((WorldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((WorldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

        // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        // extract the 3D array data to a 1D array:
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

        for (int n = 0; n < cellMix.Length; n++)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }
        return cellMix;
    }

    private int GetMainTexture(Vector3 WorldPos, Terrain terrain)
    {
        // returns the zero-based index of the most dominant texture
        // on the main terrain at this world position.
        float[] mix = GetTextureMix(WorldPos, terrain);

        float maxMix = 0;
        int maxIndex = 0;

        // loop through each mix value and find the maximum
        for (int n = 0; n < mix.Length; n++)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return maxIndex;
    }
}

public class Terraform : MonoBehaviour
{
    public Terrain terrain;
    public TerrainData tData;

    public int xRes;
    public int yRes;

    public float[,] heights;


    public Terrain myTerrain;
    public int PlayerXi;
    public int PlayerZi;

    private GameObject Player;
    void Start()
    {
        Player = GameObject.Find("Camera");

        tData = terrain.terrainData;

        xRes = tData.heightmapWidth;
        yRes = tData.heightmapHeight;

        //terrain.activeTerrain.heightmapMaximumLOD = 0;
        //something I didn't manage to translate from Java
    }

    void OnMouseDown()
    {
        // we convert float of our position x in to int
        PlayerXi = (int)Player.transform.position.x;
        // we convert float of our position z in to int
        PlayerZi = (int)Player.transform.position.z;

        // we tell heights how big the map is
        heights = tData.GetHeights(0, 0, xRes, yRes);

        // we set z in to x AND x in to y
        heights[PlayerZi, PlayerXi] = 0.01f;
        tData.SetHeights(0, 0, heights);
    }
}