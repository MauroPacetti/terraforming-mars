using UnityEngine;

/// <summary>
/// Scatters realistic deformed rocks across the Mars terrain.
/// Creates rock groups with varied sizes and colors.
/// </summary>
public class MarsRockScatterer : MonoBehaviour
{
    [Header("Settings")]
    public int rockCount = 800;
    public float scatterRadius = 500f;
    public Vector3 center = new Vector3(1000, 0, 1000);
    public float minScale = 0.1f;
    public float maxScale = 2.5f;
    public float largeRockChance = 0.05f;
    public float largeRockMaxScale = 6f;

    void Start()
    {
        ScatterRocks();
    }

    [ContextMenu("Scatter Rocks")]
    public void ScatterRocks()
    {
        // Create parent
        GameObject rocksParent = new GameObject("MarsRocks");
        rocksParent.transform.position = Vector3.zero;

        // Materials
        Material[] rockMats = CreateRockMaterials();

        // Create several base rock meshes with deformation
        Mesh[] rockMeshes = CreateRockMeshes();

        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogWarning("[RockScatterer] No terrain found!");
            return;
        }

        for (int i = 0; i < rockCount; i++)
        {
            float rx = center.x + (Random.value - 0.5f) * scatterRadius * 2;
            float rz = center.z + (Random.value - 0.5f) * scatterRadius * 2;
            float ry = terrain.SampleHeight(new Vector3(rx, 0, rz));

            // Scale with occasional large boulders
            float scale;
            if (Random.value < largeRockChance)
                scale = Random.Range(maxScale, largeRockMaxScale);
            else
                scale = Random.Range(minScale, maxScale);

            GameObject rock = new GameObject($"Rock_{i}");
            rock.transform.parent = rocksParent.transform;
            rock.transform.position = new Vector3(rx, ry + scale * 0.15f, rz);
            rock.transform.localScale = new Vector3(
                scale * Random.Range(0.6f, 1.4f),
                scale * Random.Range(0.3f, 1.0f),
                scale * Random.Range(0.6f, 1.4f)
            );
            rock.transform.rotation = Random.rotation;

            MeshFilter mf = rock.AddComponent<MeshFilter>();
            mf.mesh = rockMeshes[i % rockMeshes.Length];

            MeshRenderer mr = rock.AddComponent<MeshRenderer>();
            mr.material = rockMats[i % rockMats.Length];
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            mr.receiveShadows = true;
        }

        Debug.Log($"[RockScatterer] Placed {rockCount} rocks.");
    }

    Material[] CreateRockMaterials()
    {
        Material[] mats = new Material[4];

        // Dark basaltic
        mats[0] = new Material(Shader.Find("HDRP/Lit"));
        mats[0].SetColor("_BaseColor", new Color(0.32f, 0.26f, 0.20f));
        mats[0].SetFloat("_Metallic", 0.05f);
        mats[0].SetFloat("_Smoothness", 0.12f);

        // Sandy
        mats[1] = new Material(Shader.Find("HDRP/Lit"));
        mats[1].SetColor("_BaseColor", new Color(0.52f, 0.42f, 0.30f));
        mats[1].SetFloat("_Metallic", 0.03f);
        mats[1].SetFloat("_Smoothness", 0.15f);

        // Dark iron (hematite - common in Meridiani)
        mats[2] = new Material(Shader.Find("HDRP/Lit"));
        mats[2].SetColor("_BaseColor", new Color(0.22f, 0.18f, 0.14f));
        mats[2].SetFloat("_Metallic", 0.15f);
        mats[2].SetFloat("_Smoothness", 0.25f);

        // Reddish-brown
        mats[3] = new Material(Shader.Find("HDRP/Lit"));
        mats[3].SetColor("_BaseColor", new Color(0.42f, 0.30f, 0.22f));
        mats[3].SetFloat("_Metallic", 0.04f);
        mats[3].SetFloat("_Smoothness", 0.1f);

        return mats;
    }

    Mesh[] CreateRockMeshes()
    {
        Mesh[] meshes = new Mesh[4];

        // Different base shapes, all deformed
        meshes[0] = DeformMesh(CreateIcosphere(2), 0.35f, 42);
        meshes[1] = DeformMesh(CreateIcosphere(1), 0.45f, 73);
        meshes[2] = DeformMesh(CreateIcosphere(2), 0.3f, 101);
        meshes[3] = DeformMesh(CreateIcosphere(1), 0.5f, 29);

        return meshes;
    }

    /// <summary>
    /// Creates a subdivided icosphere mesh.
    /// </summary>
    Mesh CreateIcosphere(int subdivisions)
    {
        // Start with icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        var vertices = new System.Collections.Generic.List<Vector3>
        {
            new Vector3(-1, t, 0).normalized,
            new Vector3(1, t, 0).normalized,
            new Vector3(-1, -t, 0).normalized,
            new Vector3(1, -t, 0).normalized,
            new Vector3(0, -1, t).normalized,
            new Vector3(0, 1, t).normalized,
            new Vector3(0, -1, -t).normalized,
            new Vector3(0, 1, -t).normalized,
            new Vector3(t, 0, -1).normalized,
            new Vector3(t, 0, 1).normalized,
            new Vector3(-t, 0, -1).normalized,
            new Vector3(-t, 0, 1).normalized,
        };

        var triangles = new System.Collections.Generic.List<int>
        {
            0,11,5, 0,5,1, 0,1,7, 0,7,10, 0,10,11,
            1,5,9, 5,11,4, 11,10,2, 10,7,6, 7,1,8,
            3,9,4, 3,4,2, 3,2,6, 3,6,8, 3,8,9,
            4,9,5, 2,4,11, 6,2,10, 8,6,7, 9,8,1
        };

        // Subdivide
        for (int s = 0; s < subdivisions; s++)
        {
            var newTris = new System.Collections.Generic.List<int>();
            var midCache = new System.Collections.Generic.Dictionary<long, int>();

            for (int i = 0; i < triangles.Count; i += 3)
            {
                int a = triangles[i];
                int b = triangles[i + 1];
                int c = triangles[i + 2];

                int ab = GetMidpoint(vertices, midCache, a, b);
                int bc = GetMidpoint(vertices, midCache, b, c);
                int ca = GetMidpoint(vertices, midCache, c, a);

                newTris.AddRange(new[] { a, ab, ca });
                newTris.AddRange(new[] { b, bc, ab });
                newTris.AddRange(new[] { c, ca, bc });
                newTris.AddRange(new[] { ab, bc, ca });
            }
            triangles = newTris;
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    int GetMidpoint(System.Collections.Generic.List<Vector3> verts,
        System.Collections.Generic.Dictionary<long, int> cache, int a, int b)
    {
        long key = (long)Mathf.Min(a, b) << 32 | (long)Mathf.Max(a, b);
        if (cache.TryGetValue(key, out int mid)) return mid;

        Vector3 midpoint = ((verts[a] + verts[b]) / 2f).normalized;
        int idx = verts.Count;
        verts.Add(midpoint);
        cache[key] = idx;
        return idx;
    }

    Mesh DeformMesh(Mesh source, float amount, int seed)
    {
        Vector3[] verts = source.vertices;
        Random.InitState(seed);

        for (int i = 0; i < verts.Length; i++)
        {
            float noise = 0.7f + Random.Range(-amount, amount);
            verts[i] *= noise;
        }

        source.vertices = verts;
        source.RecalculateNormals();
        source.RecalculateBounds();
        return source;
    }
}
