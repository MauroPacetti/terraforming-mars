using UnityEngine;

public class MarsTerrainFromHeightmap : MonoBehaviour
{
    [Header("Input")]
    public Texture2D heightmap; // PNG in grayscale (Readable)
    [Range(0f, 1f)] public float heightScale01 = 0.25f; // quanto "alto" rispetto alla size
    public Vector3 terrainSize = new Vector3(600f, 120f, 600f);

    [Header("Resolution")]
    [Range(65, 2049)] public int heightmapResolution = 513; // deve essere 2^n + 1 (513, 1025, 2049)

    [Header("Material (optional)")]
    public Material terrainMat;

    [ContextMenu("Build Terrain")]
    public void BuildTerrain()
    {
        if (heightmap == null)
        {
            Debug.LogError("Assign a grayscale heightmap Texture2D.");
            return;
        }

        // Crea TerrainData
        TerrainData td = new TerrainData();
        td.heightmapResolution = heightmapResolution;
        td.size = terrainSize;

        // Campiona heightmap in una matrice [y,x]
        float[,] heights = new float[td.heightmapResolution, td.heightmapResolution];

        for (int y = 0; y < td.heightmapResolution; y++)
        {
            for (int x = 0; x < td.heightmapResolution; x++)
            {
                float u = (float)x / (td.heightmapResolution - 1);
                float v = (float)y / (td.heightmapResolution - 1);
                float h = heightmap.GetPixelBilinear(u, v).grayscale;
                heights[y, x] = h * heightScale01; // Terrain vuole 0..1
            }
        }

        td.SetHeights(0, 0, heights);

        // Crea/aggancia Terrain
        Terrain terrain = GetComponent<Terrain>();
        TerrainCollider tc = GetComponent<TerrainCollider>();

        if (terrain == null) terrain = gameObject.AddComponent<Terrain>();
        if (tc == null) tc = gameObject.AddComponent<TerrainCollider>();

        terrain.terrainData = td;
        tc.terrainData = td;

        if (terrainMat != null) terrain.materialTemplate = terrainMat;

        // Qualità base
        terrain.heightmapPixelError = 5;
        terrain.basemapDistance = 2000;
        terrain.drawInstanced = true;
    }
}
