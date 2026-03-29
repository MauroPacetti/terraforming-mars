using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// MarsSceneBuilder — Generates the entire Mars scene at runtime:
/// terrain, atmosphere, lighting, dome habitat, rover placeholder.
/// Attach to an empty GameObject and press Play.
/// </summary>
[ExecuteInEditMode]
public class MarsSceneBuilder : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int terrainWidth = 2000;
    public int terrainLength = 2000;
    public int terrainHeight = 120;
    public int heightmapRes = 513;

    [Header("Lighting")]
    public float sunIntensity = 4.5f;
    public Color sunColor = new Color(1f, 0.78f, 0.5f);
    public float sunAngleX = 35f;
    public float sunAngleY = -30f;

    [Header("Dome")]
    public float domeRadius = 12f;
    public float domeHeight = 8f;
    public Vector3 domePosition = new Vector3(1000, 0, 1000);

    [Header("Rover")]
    public Vector3 roverPosition = new Vector3(1020, 0, 985);

    void Start()
    {
        if (Application.isPlaying)
        {
            BuildScene();
        }
    }

    [ContextMenu("Build Mars Scene")]
    public void BuildScene()
    {
        CreateTerrain();
        CreateLighting();
        CreateAtmosphere();
        CreateDome();
        CreateRoverPlaceholder();
        CreatePlayer();
        CreateDustParticles();
        SetupCamera();

        Debug.Log("[MarsSceneBuilder] Scene built successfully!");
    }

    // ═══════════════════════════════════════════════
    // TERRAIN — Procedural heightmap with craters
    // ═══════════════════════════════════════════════
    void CreateTerrain()
    {
        TerrainData td = new TerrainData();
        td.heightmapResolution = heightmapRes;
        td.size = new Vector3(terrainWidth, terrainHeight, terrainLength);

        // Generate heightmap
        float[,] heights = new float[heightmapRes, heightmapRes];
        GenerateMarsHeightmap(heights);
        td.SetHeights(0, 0, heights);

        // Terrain layers (textures)
        TerrainLayer[] layers = CreateTerrainLayers();
        td.terrainLayers = layers;

        // Paint terrain
        PaintTerrain(td);

        // Create terrain object
        GameObject terrainObj = Terrain.CreateTerrainGameObject(td);
        terrainObj.name = "MarsTerrain";
        terrainObj.transform.position = Vector3.zero;

        Terrain terrain = terrainObj.GetComponent<Terrain>();
        terrain.materialTemplate = CreateTerrainMaterial();
        terrain.drawInstanced = true;
        terrain.heightmapPixelError = 5;
        terrain.detailObjectDistance = 200;
    }

    void GenerateMarsHeightmap(float[,] heights)
    {
        int res = heights.GetLength(0);

        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float u = (float)x / res;
                float v = (float)y / res;

                // Multi-octave Perlin noise for natural terrain
                float h = 0f;
                h += Mathf.PerlinNoise(u * 4f + 100, v * 4f + 100) * 0.4f;
                h += Mathf.PerlinNoise(u * 8f + 200, v * 8f + 200) * 0.2f;
                h += Mathf.PerlinNoise(u * 16f + 300, v * 16f + 300) * 0.1f;
                h += Mathf.PerlinNoise(u * 32f + 400, v * 32f + 400) * 0.05f;

                // Ridge noise for mesa formations
                float ridge = Mathf.Abs(Mathf.PerlinNoise(u * 6f + 500, v * 6f + 500) - 0.5f) * 0.4f;
                h += ridge;

                heights[y, x] = h * 0.6f + 0.1f; // Base level at 10%
            }
        }

        // Add craters
        AddCrater(heights, res, 0.25f, 0.35f, 0.08f, 0.15f);
        AddCrater(heights, res, 0.7f, 0.55f, 0.06f, 0.12f);
        AddCrater(heights, res, 0.5f, 0.15f, 0.1f, 0.18f);
        AddCrater(heights, res, 0.12f, 0.78f, 0.04f, 0.08f);
        AddCrater(heights, res, 0.85f, 0.25f, 0.035f, 0.07f);
        AddCrater(heights, res, 0.6f, 0.82f, 0.07f, 0.13f);
        AddCrater(heights, res, 0.38f, 0.62f, 0.03f, 0.06f);
        AddCrater(heights, res, 0.55f, 0.45f, 0.05f, 0.1f);

        // Flatten area around dome
        float dcx = domePosition.x / terrainWidth;
        float dcz = domePosition.z / terrainLength;
        float flatRadius = 0.04f;
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float u = (float)x / res;
                float v = (float)y / res;
                float d = Mathf.Sqrt((u - dcx) * (u - dcx) + (v - dcz) * (v - dcz));
                if (d < flatRadius)
                {
                    float blend = d / flatRadius;
                    blend = blend * blend * (3 - 2 * blend); // smoothstep
                    float flatH = heights[Mathf.RoundToInt(dcz * res), Mathf.RoundToInt(dcx * res)];
                    heights[y, x] = Mathf.Lerp(flatH, heights[y, x], blend);
                }
            }
        }
    }

    void AddCrater(float[,] heights, int res, float cx, float cy, float radius, float depth)
    {
        int minX = Mathf.Max(0, Mathf.FloorToInt((cx - radius * 2) * res));
        int maxX = Mathf.Min(res - 1, Mathf.CeilToInt((cx + radius * 2) * res));
        int minY = Mathf.Max(0, Mathf.FloorToInt((cy - radius * 2) * res));
        int maxY = Mathf.Min(res - 1, Mathf.CeilToInt((cy + radius * 2) * res));

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float u = (float)x / res;
                float v = (float)y / res;
                float dx = u - cx;
                float dy = v - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float nr = dist / radius;

                if (nr < 1f)
                {
                    // Crater bowl
                    heights[y, x] -= depth * (1f - nr * nr);
                }
                else if (nr < 1.5f)
                {
                    // Crater rim
                    float rimFactor = Mathf.Exp(-(nr - 1f) * (nr - 1f) * 8f);
                    heights[y, x] += depth * 0.3f * rimFactor;
                }
            }
        }
    }

    TerrainLayer[] CreateTerrainLayers()
    {
        // Layer 0: Base Mars sand
        TerrainLayer sandLayer = new TerrainLayer();
        sandLayer.diffuseTexture = GenerateProceduralTexture(512, "sand");
        sandLayer.normalMapTexture = GenerateProceduralNormal(512, "sand");
        sandLayer.tileSize = new Vector2(40, 40);
        sandLayer.metallic = 0f;
        sandLayer.smoothness = 0.15f;

        // Layer 1: Rocky terrain
        TerrainLayer rockLayer = new TerrainLayer();
        rockLayer.diffuseTexture = GenerateProceduralTexture(512, "rock");
        rockLayer.normalMapTexture = GenerateProceduralNormal(512, "rock");
        rockLayer.tileSize = new Vector2(25, 25);
        rockLayer.metallic = 0.05f;
        rockLayer.smoothness = 0.25f;

        // Layer 2: Dark gravel
        TerrainLayer gravelLayer = new TerrainLayer();
        gravelLayer.diffuseTexture = GenerateProceduralTexture(512, "gravel");
        gravelLayer.normalMapTexture = GenerateProceduralNormal(512, "gravel");
        gravelLayer.tileSize = new Vector2(15, 15);
        gravelLayer.metallic = 0.02f;
        gravelLayer.smoothness = 0.1f;

        return new TerrainLayer[] { sandLayer, rockLayer, gravelLayer };
    }

    void PaintTerrain(TerrainData td)
    {
        int alphamapRes = td.alphamapResolution;
        float[,,] alphamaps = new float[alphamapRes, alphamapRes, 3];

        for (int y = 0; y < alphamapRes; y++)
        {
            for (int x = 0; x < alphamapRes; x++)
            {
                float u = (float)x / alphamapRes;
                float v = (float)y / alphamapRes;

                float noise1 = Mathf.PerlinNoise(u * 20 + 50, v * 20 + 50);
                float noise2 = Mathf.PerlinNoise(u * 40 + 80, v * 40 + 80);
                float slope = td.GetSteepness(u, v) / 90f;

                // Base sand everywhere
                float sand = 1f - slope * 2f;
                // Rock on slopes
                float rock = Mathf.Clamp01(slope * 3f + noise2 * 0.3f - 0.2f);
                // Gravel patches
                float gravel = Mathf.Clamp01(noise1 - 0.55f) * 2f;

                // Normalize
                float total = sand + rock + gravel;
                if (total > 0)
                {
                    alphamaps[y, x, 0] = Mathf.Max(0, sand) / total;
                    alphamaps[y, x, 1] = Mathf.Max(0, rock) / total;
                    alphamaps[y, x, 2] = Mathf.Max(0, gravel) / total;
                }
                else
                {
                    alphamaps[y, x, 0] = 1f;
                }
            }
        }

        td.SetAlphamaps(0, 0, alphamaps);
    }

    Material CreateTerrainMaterial()
    {
        // Use HDRP Lit shader for terrain
        Material mat = new Material(Shader.Find("HDRP/Lit"));
        mat.SetColor("_BaseColor", new Color(0.75f, 0.6f, 0.42f));
        return mat;
    }

    // ═══════════════════════════════════════════════
    // PROCEDURAL TEXTURES
    // ═══════════════════════════════════════════════
    Texture2D GenerateProceduralTexture(int size, string type)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float u = (float)x / size;
                float v = (float)y / size;
                Color c;

                switch (type)
                {
                    case "sand":
                        float n = Mathf.PerlinNoise(u * 30, v * 30) * 0.15f;
                        float n2 = Mathf.PerlinNoise(u * 60 + 10, v * 60 + 10) * 0.08f;
                        float baseVal = 0.55f + n + n2;
                        c = new Color(baseVal * 1.1f, baseVal * 0.85f, baseVal * 0.58f);
                        break;

                    case "rock":
                        float rn = Mathf.PerlinNoise(u * 25, v * 25) * 0.2f;
                        float rn2 = Mathf.PerlinNoise(u * 50 + 5, v * 50 + 5) * 0.1f;
                        float rockBase = 0.38f + rn + rn2;
                        c = new Color(rockBase * 0.95f, rockBase * 0.75f, rockBase * 0.52f);
                        break;

                    case "gravel":
                        float gn = Mathf.PerlinNoise(u * 40, v * 40) * 0.15f;
                        float gn2 = Mathf.PerlinNoise(u * 80 + 20, v * 80 + 20) * 0.1f;
                        float gravelBase = 0.3f + gn + gn2;
                        c = new Color(gravelBase * 0.9f, gravelBase * 0.72f, gravelBase * 0.48f);
                        break;

                    default:
                        c = Color.gray;
                        break;
                }

                pixels[y * size + x] = c;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply(true);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Bilinear;
        return tex;
    }

    Texture2D GenerateProceduralNormal(int size, string type)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, true, true);
        Color[] pixels = new Color[size * size];

        float scale = type == "rock" ? 25f : type == "gravel" ? 40f : 30f;
        float strength = type == "rock" ? 1.2f : 0.6f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float u = (float)x / size;
                float v = (float)y / size;
                float s = 1f / size;

                float hc = Mathf.PerlinNoise(u * scale, v * scale);
                float hr = Mathf.PerlinNoise((u + s) * scale, v * scale);
                float hu = Mathf.PerlinNoise(u * scale, (v + s) * scale);

                float dx = (hr - hc) * strength;
                float dy = (hu - hc) * strength;

                pixels[y * size + x] = new Color(0.5f + dx * 0.5f, 0.5f + dy * 0.5f, 1f, 1f);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply(true);
        tex.wrapMode = TextureWrapMode.Repeat;
        return tex;
    }

    // ═══════════════════════════════════════════════
    // LIGHTING — Martian sun and ambient
    // ═══════════════════════════════════════════════
    void CreateLighting()
    {
        // Main directional light (Sun)
        GameObject sunObj = new GameObject("Mars Sun");
        Light sunLight = sunObj.AddComponent<Light>();
        sunLight.type = LightType.Directional;
        sunLight.color = sunColor;
        sunLight.intensity = sunIntensity;
        sunLight.shadows = LightShadows.Soft;
        sunObj.transform.rotation = Quaternion.Euler(sunAngleX, sunAngleY, 0);

        // Configure HDRP light data
        HDAdditionalLightData hdLight = sunObj.GetComponent<HDAdditionalLightData>();
        if (hdLight == null) hdLight = sunObj.AddComponent<HDAdditionalLightData>();
        hdLight.SetIntensity(80000f, LightUnit.Lux); // Realistic Mars illumination
        hdLight.EnableShadows(true);
        hdLight.SetShadowResolution(2048);
        hdLight.volumetricDimmer = 1f;

        // Fill light (bounce from terrain)
        GameObject fillObj = new GameObject("Fill Light");
        Light fillLight = fillObj.AddComponent<Light>();
        fillLight.type = LightType.Directional;
        fillLight.color = new Color(0.7f, 0.55f, 0.35f);
        fillLight.intensity = 0.8f;
        fillLight.shadows = LightShadows.None;
        fillObj.transform.rotation = Quaternion.Euler(60, 150, 0);

        HDAdditionalLightData hdFill = fillObj.GetComponent<HDAdditionalLightData>();
        if (hdFill == null) hdFill = fillObj.AddComponent<HDAdditionalLightData>();
        hdFill.SetIntensity(15000f, LightUnit.Lux);
    }

    // ═══════════════════════════════════════════════
    // ATMOSPHERE — HDRP Volume Profile for Mars
    // ═══════════════════════════════════════════════
    void CreateAtmosphere()
    {
        // Global volume
        GameObject volObj = new GameObject("Mars Atmosphere");
        Volume volume = volObj.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 1;

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        volume.profile = profile;

        // Visual Environment
        VisualEnvironment ve = profile.Add<VisualEnvironment>(true);
        ve.skyType.Override((int)SkyType.Gradient);
        ve.skyAmbientMode.Override(SkyAmbientMode.Dynamic);

        // Gradient Sky — Mars colors
        GradientSky sky = profile.Add<GradientSky>(true);
        sky.top.Override(new Color(0.12f, 0.08f, 0.04f));        // Dark brown-black overhead
        sky.middle.Override(new Color(0.62f, 0.42f, 0.22f));     // Butterscotch horizon
        sky.bottom.Override(new Color(0.35f, 0.25f, 0.15f));     // Sandy ground
        sky.gradientDiffusion.Override(1.5f);
        sky.exposure.Override(1.2f);

        // Fog — Martian dust haze
        Fog fog = profile.Add<Fog>(true);
        fog.enabled.Override(true);
        fog.meanFreePath.Override(800f);
        fog.baseHeight.Override(0f);
        fog.maximumHeight.Override(150f);
        fog.albedo.Override(new Color(0.65f, 0.48f, 0.28f)); // Sandy dust
        fog.enableVolumetricFog.Override(true);
        fog.anisotropy.Override(0.6f);

        // Bloom
        Bloom bloom = profile.Add<Bloom>(true);
        bloom.intensity.Override(0.3f);
        bloom.threshold.Override(1.2f);
        bloom.scatter.Override(0.65f);
        bloom.tint.Override(new Color(1f, 0.85f, 0.65f)); // Warm Mars tint

        // Color Adjustments
        ColorAdjustments ca = profile.Add<ColorAdjustments>(true);
        ca.postExposure.Override(0.5f);
        ca.contrast.Override(15f);
        ca.saturation.Override(-10f); // Slightly desaturated
        ca.colorFilter.Override(new Color(1f, 0.92f, 0.82f)); // Warm filter

        // Vignette
        Vignette vignette = profile.Add<Vignette>(true);
        vignette.intensity.Override(0.3f);
        vignette.smoothness.Override(0.4f);

        // Ambient Occlusion
        ScreenSpaceAmbientOcclusion ao = profile.Add<ScreenSpaceAmbientOcclusion>(true);
        ao.intensity.Override(0.8f);
        ao.radius.Override(2f);

        // Exposure control
        Exposure exposure = profile.Add<Exposure>(true);
        exposure.mode.Override(ExposureMode.Fixed);
        exposure.fixedExposure.Override(10f);

        Debug.Log("[MarsSceneBuilder] Atmosphere configured.");
    }

    // ═══════════════════════════════════════════════
    // DOME — Glass geodesic habitat
    // ═══════════════════════════════════════════════
    void CreateDome()
    {
        // Get terrain height at dome position
        float terrainY = GetTerrainHeight(domePosition.x, domePosition.z);
        Vector3 pos = new Vector3(domePosition.x, terrainY, domePosition.z);

        GameObject domeParent = new GameObject("Panopticon Dome");
        domeParent.transform.position = pos;

        // Glass hemisphere
        GameObject domeGlass = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        domeGlass.name = "DomeGlass";
        domeGlass.transform.parent = domeParent.transform;
        domeGlass.transform.localPosition = Vector3.zero;
        domeGlass.transform.localScale = new Vector3(domeRadius * 2, domeHeight, domeRadius * 2);

        // HDRP Glass material
        Material glassMat = new Material(Shader.Find("HDRP/Lit"));
        glassMat.SetFloat("_SurfaceType", 1); // Transparent
        glassMat.SetFloat("_BlendMode", 0); // Alpha
        glassMat.SetColor("_BaseColor", new Color(0.85f, 0.9f, 0.95f, 0.08f));
        glassMat.SetFloat("_Metallic", 0.0f);
        glassMat.SetFloat("_Smoothness", 0.95f);
        glassMat.SetFloat("_IOR", 1.5f);
        glassMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        glassMat.EnableKeyword("_BLENDMODE_ALPHA");
        glassMat.renderQueue = 3000;
        domeGlass.GetComponent<Renderer>().material = glassMat;

        // Steel structure ribs (meridians)
        Material steelMat = new Material(Shader.Find("HDRP/Lit"));
        steelMat.SetColor("_BaseColor", new Color(0.7f, 0.7f, 0.72f));
        steelMat.SetFloat("_Metallic", 0.9f);
        steelMat.SetFloat("_Smoothness", 0.4f);

        for (int i = 0; i < 16; i++)
        {
            float angle = (float)i / 16 * Mathf.PI * 2;
            CreateDomeRib(domeParent.transform, angle, steelMat);
        }

        // Latitude rings
        for (int ring = 1; ring <= 5; ring++)
        {
            float phi = (float)ring / 6 * 90f * Mathf.Deg2Rad;
            float ringRadius = Mathf.Cos(phi) * domeRadius;
            float ringY = Mathf.Sin(phi) * domeHeight * 0.5f;

            GameObject ringObj = new GameObject($"LatRing_{ring}");
            ringObj.transform.parent = domeParent.transform;
            ringObj.transform.localPosition = new Vector3(0, ringY, 0);

            // Create torus-like ring from cylinders
            int segments = 48;
            for (int s = 0; s < segments; s++)
            {
                float a1 = (float)s / segments * Mathf.PI * 2;
                float a2 = (float)(s + 1) / segments * Mathf.PI * 2;
                Vector3 p1 = new Vector3(Mathf.Cos(a1) * ringRadius, 0, Mathf.Sin(a1) * ringRadius);
                Vector3 p2 = new Vector3(Mathf.Cos(a2) * ringRadius, 0, Mathf.Sin(a2) * ringRadius);

                GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                seg.name = $"RingSeg_{ring}_{s}";
                seg.transform.parent = ringObj.transform;
                seg.transform.localPosition = (p1 + p2) / 2;
                float len = Vector3.Distance(p1, p2);
                seg.transform.localScale = new Vector3(0.04f, len / 2, 0.04f);
                seg.transform.up = (p2 - p1).normalized;
                seg.GetComponent<Renderer>().material = steelMat;
                Destroy(seg.GetComponent<Collider>());
            }
        }

        // Base ring
        CreateBaseRing(domeParent.transform, steelMat);

        // Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        floor.name = "DomeFloor";
        floor.transform.parent = domeParent.transform;
        floor.transform.localPosition = new Vector3(0, 0.05f, 0);
        floor.transform.localScale = new Vector3(domeRadius * 1.9f, 0.1f, domeRadius * 1.9f);

        Material floorMat = new Material(Shader.Find("HDRP/Lit"));
        floorMat.SetColor("_BaseColor", new Color(0.78f, 0.75f, 0.72f));
        floorMat.SetFloat("_Metallic", 0.05f);
        floorMat.SetFloat("_Smoothness", 0.45f);
        floor.GetComponent<Renderer>().material = floorMat;

        // Interior lights
        for (int i = 0; i < 6; i++)
        {
            float a = (float)i / 6 * Mathf.PI * 2;
            GameObject lightObj = new GameObject($"InteriorLight_{i}");
            lightObj.transform.parent = domeParent.transform;
            lightObj.transform.localPosition = new Vector3(
                Mathf.Cos(a) * domeRadius * 0.5f,
                domeHeight * 0.6f,
                Mathf.Sin(a) * domeRadius * 0.5f
            );

            Light pl = lightObj.AddComponent<Light>();
            pl.type = LightType.Point;
            pl.color = new Color(1f, 0.88f, 0.7f);
            pl.range = 15f;

            HDAdditionalLightData hdPl = lightObj.GetComponent<HDAdditionalLightData>();
            if (hdPl == null) hdPl = lightObj.AddComponent<HDAdditionalLightData>();
            hdPl.SetIntensity(800f, LightUnit.Lumen);
        }

        Debug.Log("[MarsSceneBuilder] Dome created.");
    }

    void CreateDomeRib(Transform parent, float angle, Material mat)
    {
        int segments = 20;
        for (int i = 0; i < segments; i++)
        {
            float t1 = (float)i / segments * 90f * Mathf.Deg2Rad;
            float t2 = (float)(i + 1) / segments * 90f * Mathf.Deg2Rad;

            Vector3 p1 = new Vector3(
                Mathf.Cos(angle) * Mathf.Cos(t1) * domeRadius,
                Mathf.Sin(t1) * domeHeight * 0.5f,
                Mathf.Sin(angle) * Mathf.Cos(t1) * domeRadius
            );
            Vector3 p2 = new Vector3(
                Mathf.Cos(angle) * Mathf.Cos(t2) * domeRadius,
                Mathf.Sin(t2) * domeHeight * 0.5f,
                Mathf.Sin(angle) * Mathf.Cos(t2) * domeRadius
            );

            GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            seg.name = $"Rib_{angle:F1}_{i}";
            seg.transform.parent = parent;
            seg.transform.localPosition = (p1 + p2) / 2;
            float len = Vector3.Distance(p1, p2);
            seg.transform.localScale = new Vector3(0.06f, len / 2, 0.06f);
            seg.transform.up = (p2 - p1).normalized;
            seg.GetComponent<Renderer>().material = mat;
            Destroy(seg.GetComponent<Collider>());
        }
    }

    void CreateBaseRing(Transform parent, Material mat)
    {
        int segments = 64;
        for (int s = 0; s < segments; s++)
        {
            float a1 = (float)s / segments * Mathf.PI * 2;
            float a2 = (float)(s + 1) / segments * Mathf.PI * 2;
            Vector3 p1 = new Vector3(Mathf.Cos(a1) * domeRadius, 0, Mathf.Sin(a1) * domeRadius);
            Vector3 p2 = new Vector3(Mathf.Cos(a2) * domeRadius, 0, Mathf.Sin(a2) * domeRadius);

            GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            seg.name = $"BaseRing_{s}";
            seg.transform.parent = parent;
            seg.transform.localPosition = (p1 + p2) / 2;
            float len = Vector3.Distance(p1, p2);
            seg.transform.localScale = new Vector3(0.3f, len / 2, 0.3f);
            seg.transform.up = (p2 - p1).normalized;
            seg.GetComponent<Renderer>().material = mat;
            Destroy(seg.GetComponent<Collider>());
        }
    }

    // ═══════════════════════════════════════════════
    // ROVER PLACEHOLDER
    // ═══════════════════════════════════════════════
    void CreateRoverPlaceholder()
    {
        float terrainY = GetTerrainHeight(roverPosition.x, roverPosition.z);
        Vector3 pos = new Vector3(roverPosition.x, terrainY, roverPosition.z);

        GameObject roverParent = new GameObject("Rover");
        roverParent.transform.position = pos;

        Material armorMat = new Material(Shader.Find("HDRP/Lit"));
        armorMat.SetColor("_BaseColor", new Color(0.4f, 0.38f, 0.36f));
        armorMat.SetFloat("_Metallic", 0.75f);
        armorMat.SetFloat("_Smoothness", 0.4f);

        Material tireMat = new Material(Shader.Find("HDRP/Lit"));
        tireMat.SetColor("_BaseColor", new Color(0.08f, 0.08f, 0.08f));
        tireMat.SetFloat("_Metallic", 0.02f);
        tireMat.SetFloat("_Smoothness", 0.15f);

        // Main body
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "RoverBody";
        body.transform.parent = roverParent.transform;
        body.transform.localPosition = new Vector3(0, 0.9f, 0);
        body.transform.localScale = new Vector3(2.2f, 0.8f, 3.2f);
        body.GetComponent<Renderer>().material = armorMat;

        // Upper cabin
        GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cabin.name = "RoverCabin";
        cabin.transform.parent = roverParent.transform;
        cabin.transform.localPosition = new Vector3(0, 1.5f, -0.2f);
        cabin.transform.localScale = new Vector3(1.8f, 0.6f, 2.0f);
        cabin.GetComponent<Renderer>().material = armorMat;

        // 4 Wheels
        float wheelRadius = 0.45f;
        Vector3[] wheelPositions = {
            new Vector3(-1.1f, wheelRadius, -1.1f),
            new Vector3(1.1f, wheelRadius, -1.1f),
            new Vector3(-1.1f, wheelRadius, 1.1f),
            new Vector3(1.1f, wheelRadius, 1.1f)
        };

        foreach (Vector3 wp in wheelPositions)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = "Wheel";
            wheel.transform.parent = roverParent.transform;
            wheel.transform.localPosition = wp;
            wheel.transform.localScale = new Vector3(wheelRadius * 2, 0.2f, wheelRadius * 2);
            wheel.transform.rotation = Quaternion.Euler(0, 0, 90);
            wheel.GetComponent<Renderer>().material = tireMat;
        }

        // Headlights
        for (int s = -1; s <= 1; s += 2)
        {
            GameObject hlObj = new GameObject($"Headlight_{s}");
            hlObj.transform.parent = roverParent.transform;
            hlObj.transform.localPosition = new Vector3(s * 0.7f, 1.0f, -1.7f);
            hlObj.transform.localRotation = Quaternion.Euler(10, 0, 0);

            Light hl = hlObj.AddComponent<Light>();
            hl.type = LightType.Spot;
            hl.color = new Color(1f, 0.95f, 0.85f);
            hl.spotAngle = 50;
            hl.range = 40;

            HDAdditionalLightData hdHl = hlObj.GetComponent<HDAdditionalLightData>();
            if (hdHl == null) hdHl = hlObj.AddComponent<HDAdditionalLightData>();
            hdHl.SetIntensity(5000f, LightUnit.Lumen);
        }

        Debug.Log("[MarsSceneBuilder] Rover placeholder created.");
    }

    // ═══════════════════════════════════════════════
    // PLAYER — First person controller
    // ═══════════════════════════════════════════════
    void CreatePlayer()
    {
        float terrainY = GetTerrainHeight(domePosition.x, domePosition.z);

        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(domePosition.x, terrainY + 1.8f, domePosition.z);

        // Character controller
        CharacterController cc = player.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.3f;
        cc.center = new Vector3(0, 0.9f, 0);

        // Camera
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("MainCamera");
            camObj.tag = "MainCamera";
            cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<HDAdditionalCameraData>();
        }
        cam.transform.parent = player.transform;
        cam.transform.localPosition = new Vector3(0, 1.6f, 0);
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 5000f;

        // Add FPS controller
        player.AddComponent<FPSController>();

        Debug.Log("[MarsSceneBuilder] Player created.");
    }

    // ═══════════════════════════════════════════════
    // DUST PARTICLES
    // ═══════════════════════════════════════════════
    void CreateDustParticles()
    {
        GameObject dustObj = new GameObject("MarsDust");
        dustObj.transform.position = domePosition + Vector3.up * 30;

        ParticleSystem ps = dustObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.maxParticles = 5000;
        main.startLifetime = 15f;
        main.startSpeed = 2f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.3f);
        main.startColor = new Color(0.7f, 0.55f, 0.35f, 0.15f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = true;
        main.gravityModifier = -0.01f;

        var emission = ps.emission;
        emission.rateOverTime = 300;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(400, 60, 400);

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.x = new ParticleSystem.MinMaxCurve(1f, 4f);
        velocity.y = new ParticleSystem.MinMaxCurve(-0.3f, 0.5f);

        var renderer = dustObj.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("HDRP/Unlit"));
        renderer.material.SetColor("_UnlitColor", new Color(0.7f, 0.55f, 0.35f, 0.12f));

        Debug.Log("[MarsSceneBuilder] Dust particles created.");
    }

    // ═══════════════════════════════════════════════
    // CAMERA SETUP
    // ═══════════════════════════════════════════════
    void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.fieldOfView = 70;
            cam.allowHDR = true;
        }
    }

    // ═══════════════════════════════════════════════
    // UTILITY
    // ═══════════════════════════════════════════════
    float GetTerrainHeight(float x, float z)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            return terrain.SampleHeight(new Vector3(x, 0, z));
        }
        return 0f;
    }
}
