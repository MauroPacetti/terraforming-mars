using UnityEngine;

/// <summary>
/// Mars HUD overlay — displays coordinates, time, temperature, energy.
/// </summary>
public class MarsHUD : MonoBehaviour
{
    private MarsDayNightCycle dayNight;
    private GUIStyle hudStyle;
    private GUIStyle titleStyle;
    private GUIStyle centerStyle;

    [Header("Settings")]
    public float energy = 78f;

    void Start()
    {
        dayNight = FindFirstObjectByType<MarsDayNightCycle>();
    }

    void OnGUI()
    {
        if (hudStyle == null)
        {
            hudStyle = new GUIStyle(GUI.skin.label);
            hudStyle.fontSize = 12;
            hudStyle.normal.textColor = new Color(0.33f, 0.8f, 1f);
            hudStyle.font = Font.CreateDynamicFontFromOSFont("Consolas", 12);

            titleStyle = new GUIStyle(hudStyle);
            titleStyle.fontSize = 14;
            titleStyle.fontStyle = FontStyle.Bold;

            centerStyle = new GUIStyle(hudStyle);
            centerStyle.alignment = TextAnchor.MiddleCenter;
            centerStyle.fontSize = 10;
            centerStyle.normal.textColor = new Color(0.27f, 0.53f, 0.6f);
        }

        // Background panels
        GUI.color = new Color(0, 0, 0, 0.7f);
        GUI.DrawTexture(new Rect(10, 10, 200, 170), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(Screen.width - 180, 10, 170, 150), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // ── LEFT PANEL ──
        float y = 15;
        GUI.Label(new Rect(15, y, 190, 20), "PANOPTICON MODULE", titleStyle);
        y += 22;

        // Draw separator
        GUI.color = new Color(0, 0.7f, 1f, 0.3f);
        GUI.DrawTexture(new Rect(15, y, 180, 1), Texture2D.whiteTexture);
        GUI.color = Color.white;
        y += 5;

        Vector3 pos = Camera.main != null ? Camera.main.transform.position : Vector3.zero;

        // Convert Unity position to approximate Mars lat/lon
        double lat = MarsRealData.LandingLat + (pos.z / MarsRealData.MetersPerDegree);
        double lon = MarsRealData.LandingLonE + (pos.x / MarsRealData.MetersPerDegree);

        GUI.Label(new Rect(15, y, 190, 18), $"LAT: {lat:F4} S", hudStyle); y += 18;
        GUI.Label(new Rect(15, y, 190, 18), $"LON: {lon:F4} E", hudStyle); y += 18;
        GUI.Label(new Rect(15, y, 190, 18), $"ALT: {pos.y:F1}m", hudStyle); y += 18;

        float temp = dayNight != null ? dayNight.GetTemperature() : -63f;
        GUI.Label(new Rect(15, y, 190, 18), $"TEMP: {temp:F0} C", hudStyle); y += 18;
        GUI.Label(new Rect(15, y, 190, 18), "PRESSURE: 636 Pa", hudStyle); y += 18;
        GUI.Label(new Rect(15, y, 190, 18), "SITE: MERIDIANI PLANUM", hudStyle);

        // ── RIGHT PANEL ──
        float rx = Screen.width - 175;
        y = 15;
        string time = dayNight != null ? dayNight.GetTimeString() : "06:00";
        GUI.Label(new Rect(rx, y, 160, 20), "SOL 001", titleStyle); y += 22;
        GUI.color = new Color(0, 0.7f, 1f, 0.3f);
        GUI.DrawTexture(new Rect(rx, y, 150, 1), Texture2D.whiteTexture);
        GUI.color = Color.white;
        y += 5;

        GUI.Label(new Rect(rx, y, 160, 18), $"TIME: {time}", hudStyle); y += 18;
        GUI.Label(new Rect(rx, y, 160, 18), "WIND: 12 km/h", hudStyle); y += 18;
        GUI.Label(new Rect(rx, y, 160, 18), "DUST: LOW", hudStyle); y += 18;
        GUI.Label(new Rect(rx, y, 160, 18), "VIS: 18 km", hudStyle); y += 18;
        GUI.Label(new Rect(rx, y, 160, 18), "ROVER: IDLE", hudStyle);

        // ── ENERGY BAR (top center) ──
        float barW = 260, barH = 16;
        float barX = (Screen.width - barW) / 2;
        GUI.color = new Color(0, 0, 0, 0.65f);
        GUI.DrawTexture(new Rect(barX, 12, barW, barH), Texture2D.whiteTexture);
        GUI.color = new Color(1f, 0.47f, 0f, 0.85f);
        GUI.DrawTexture(new Rect(barX + 2, 14, (barW - 4) * (energy / 100f), barH - 4), Texture2D.whiteTexture);
        GUI.color = Color.white;

        centerStyle.normal.textColor = new Color(0.8f, 0.53f, 0.27f);
        GUI.Label(new Rect(barX, 30, barW, 18), $"ENERGY: {energy:F0}/100 kWh", centerStyle);

        // ── BOTTOM CENTER ──
        centerStyle.normal.textColor = new Color(0.27f, 0.53f, 0.6f);
        GUI.Label(new Rect(0, Screen.height - 35, Screen.width, 25),
            "[WASD] Move   [Mouse] Look   [Shift] Run   [Space] Jump   [Esc] Release", centerStyle);

        // ── CROSSHAIR ──
        float cx = Screen.width / 2f, cy = Screen.height / 2f;
        GUI.color = new Color(0, 0.7f, 1f, 0.25f);
        GUI.DrawTexture(new Rect(cx - 0.5f, cy - 14, 1, 28), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(cx - 14, cy - 0.5f, 28, 1), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
}
