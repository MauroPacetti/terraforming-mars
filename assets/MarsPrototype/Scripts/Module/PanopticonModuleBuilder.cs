using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PanopticonModuleBuilder : MonoBehaviour
{
    [Header("Dimensions")]
    public float radius = 6.0f;           // raggio stanza
    public float height = 3.2f;           // altezza pareti
    public float wallThickness = 0.25f;
    public int segments = 24;             // più segmenti = più finestre potenziali

    [Header("Windows")]
    public int windowsCount = 12;         // quante finestre/oblò
    public float windowWidth = 1.6f;
    public float windowHeight = 1.1f;
    public float windowBottom = 1.1f;     // altezza dal pavimento
    public float windowFrameThickness = 0.12f;
    public bool usePortholes = false;     // se true: oblò circolari
    public float portholeRadius = 0.6f;

    [Header("Materials (optional)")]
    public Material wallMat;
    public Material glassMat;
    public Material frameMat;
    public Material floorMat;
    public Material ceilingMat;

    [Header("Rebuild")]
    public bool rebuildNow;

    private const string ROOT_NAME = "_PANOPTICON_BUILT";

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (rebuildNow)
            {
                rebuildNow = false;
                Rebuild();
            }
        }
    }

    [ContextMenu("Rebuild Panopticon")]
    public void Rebuild()
    {
        // cleanup
        var old = transform.Find(ROOT_NAME);
        if (old != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(old.gameObject);
#else
            Destroy(old.gameObject);
#endif
        }

        var root = new GameObject(ROOT_NAME);
        root.transform.SetParent(transform, false);

        BuildFloor(root.transform);
        BuildCeiling(root.transform);
        BuildWallsAndWindows(root.transform);
    }

    void BuildFloor(Transform parent)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = "Floor";
        go.transform.SetParent(parent, false);

        // Unity cylinder: radius 0.5, height 2
        go.transform.localScale = new Vector3(radius * 2f, 0.08f, radius * 2f);
        go.transform.localPosition = new Vector3(0, 0, 0);

        ApplyMat(go, floorMat);
    }

    void BuildCeiling(Transform parent)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = "Ceiling";
        go.transform.SetParent(parent, false);

        go.transform.localScale = new Vector3(radius * 2f, 0.06f, radius * 2f);
        go.transform.localPosition = new Vector3(0, height, 0);

        ApplyMat(go, ceilingMat);
    }

    void BuildWallsAndWindows(Transform parent)
    {
        // Pareti segmentate in "spicchi"
        float angleStep = 360f / Mathf.Max(3, segments);
        var windowAngles = PickWindowAngles(windowsCount, segments);

        for (int i = 0; i < segments; i++)
        {
            float a0 = i * angleStep;
            float aMid = a0 + angleStep * 0.5f;

            // segmento muro (box)
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = $"WallSeg_{i:00}";
            wall.transform.SetParent(parent, false);

            float arc = Mathf.Deg2Rad * angleStep * radius;
            float segWidth = arc; // approssimazione della larghezza del segmento

            wall.transform.localScale = new Vector3(segWidth, height, wallThickness);

            Vector3 pos = Polar(radius, aMid);
            wall.transform.localPosition = new Vector3(pos.x, height * 0.5f, pos.z);
            wall.transform.localRotation = Quaternion.Euler(0, -aMid, 0);

            ApplyMat(wall, wallMat);

            bool hasWindow = windowAngles.Contains(i);
            if (hasWindow)
            {
                if (usePortholes) BuildPorthole(parent, aMid, segWidth);
                else BuildWindow(parent, aMid, segWidth);
                // "buchiamo" visivamente il muro mettendo una seconda parete a sinistra/destra
                // (senza boolean ops: è un trucco per prototipo)
                HideWallCenter(wall, segWidth);
            }
        }
    }

    void HideWallCenter(GameObject wall, float segWidth)
    {
        // Riduciamo la parete centrale così "sembra" che ci sia un foro finestra
        // (soluzione rapida: in produzione si usano mesh con buchi)
        var s = wall.transform.localScale;
        s.x = Mathf.Max(0.2f, segWidth * 0.35f);
        wall.transform.localScale = s;
    }

    void BuildWindow(Transform parent, float angleDeg, float segWidth)
    {
        Vector3 basePos = Polar(radius - wallThickness * 0.5f, angleDeg);

        // frame esterno
        var frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frame.name = $"WindowFrame_{angleDeg:0}";
        frame.transform.SetParent(parent, false);

        frame.transform.localScale = new Vector3(windowWidth + windowFrameThickness, windowHeight + windowFrameThickness, windowFrameThickness);
        frame.transform.localPosition = new Vector3(basePos.x, windowBottom + windowHeight * 0.5f, basePos.z);
        frame.transform.localRotation = Quaternion.Euler(0, -angleDeg, 0);
        ApplyMat(frame, frameMat);

        // glass (leggermente più fuori)
        var glass = GameObject.CreatePrimitive(PrimitiveType.Cube);
        glass.name = $"WindowGlass_{angleDeg:0}";
        glass.transform.SetParent(parent, false);

        glass.transform.localScale = new Vector3(windowWidth, windowHeight, 0.03f);
        glass.transform.localPosition = frame.transform.localPosition + frame.transform.forward * 0.03f;
        glass.transform.localRotation = frame.transform.localRotation;

        // Glass: collider opzionale (serve per raycast "clicca fuori")
        // Lasciamo il BoxCollider attivo.
        ApplyMat(glass, glassMat);
    }

    void BuildPorthole(Transform parent, float angleDeg, float segWidth)
    {
        Vector3 basePos = Polar(radius - wallThickness * 0.5f, angleDeg);

        // frame (cilindro schiacciato)
        var frame = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        frame.name = $"PortholeFrame_{angleDeg:0}";
        frame.transform.SetParent(parent, false);

        frame.transform.localScale = new Vector3((portholeRadius * 2f + windowFrameThickness), 0.06f, (portholeRadius * 2f + windowFrameThickness));
        frame.transform.localPosition = new Vector3(basePos.x, windowBottom + portholeRadius, basePos.z);
        frame.transform.localRotation = Quaternion.Euler(90, -angleDeg, 0);
        ApplyMat(frame, frameMat);

        var glass = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        glass.name = $"PortholeGlass_{angleDeg:0}";
        glass.transform.SetParent(parent, false);

        glass.transform.localScale = new Vector3(portholeRadius * 2f, 0.02f, portholeRadius * 2f);
        glass.transform.localPosition = frame.transform.localPosition + frame.transform.up * 0.07f;
        glass.transform.localRotation = frame.transform.localRotation;
        ApplyMat(glass, glassMat);
    }

    HashSet<int> PickWindowAngles(int count, int segs)
    {
        var set = new HashSet<int>();
        count = Mathf.Clamp(count, 0, segs);

        // Distribuzione quasi uniforme
        float step = (float)segs / Mathf.Max(1, count);
        for (int i = 0; i < count; i++)
        {
            int idx = Mathf.RoundToInt(i * step) % segs;
            set.Add(idx);
        }
        return set;
    }

    static Vector3 Polar(float r, float angleDeg)
    {
        float a = angleDeg * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(a) * r, 0f, Mathf.Cos(a) * r);
    }

    static void ApplyMat(GameObject go, Material mat)
    {
        if (mat == null) return;
        var mr = go.GetComponent<MeshRenderer>();
        if (mr != null) mr.sharedMaterial = mat;
    }
}
