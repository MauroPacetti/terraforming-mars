using UnityEngine;

/// <summary>
/// Mars Real Data — Coordinate system and real-world reference data
/// Based on Opportunity landing site at Meridiani Planum.
///
/// COORDINATE SYSTEM: MOLA / IAU 2000
/// - Lat (planetocentric): 1.948282 S
/// - Lon (East-positive, 0-360): 354.47417 E
///
/// AOI: 10x10 km bounding box centered on Eagle Crater
/// - Lat: [-2.0328, -1.8638]
/// - LonE: [354.3897, 354.5587]
///
/// DATA SOURCES:
/// - DEM base: MOLA MEGDR (128 ppd / 463 mpp)
/// - Imagery context: CTX mosaic 6 m/px (USGS / Murray Lab)
/// - Hi-res: HiRISE ESP_050177_1780
/// - Eagle Crater: ~22m diameter, ~3m depth
///
/// In Unity: 1 Unity unit = 1 meter
/// Map center is at Unity origin (0,0,0)
/// </summary>
public static class MarsRealData
{
    // ═══════════════════════════════════════════════
    // COORDINATE CONSTANTS
    // ═══════════════════════════════════════════════

    // Landing site (planetocentric)
    public const double LandingLat = -1.948282;    // South
    public const double LandingLonE = 354.47417;   // East-positive

    // Mars radius
    public const double MarsRadius = 3389500.0;    // meters
    public const double MetersPerDegree = 59200.0;  // approx at equator

    // AOI Bounding Box (10x10 km)
    public const double AOI_LatMin = -2.0328;
    public const double AOI_LatMax = -1.8638;
    public const double AOI_LonMin = 354.3897;
    public const double AOI_LonMax = 354.5587;

    // AOI size in meters
    public const float AOI_Width = 10000f;   // 10 km East-West
    public const float AOI_Length = 10000f;   // 10 km North-South

    // Eagle Crater
    public const float EagleCraterDiameter = 22f;  // meters
    public const float EagleCraterDepth = 3f;      // meters

    // ═══════════════════════════════════════════════
    // PHYSICAL CONSTANTS
    // ═══════════════════════════════════════════════

    public const float MarsGravity = 3.72f;         // m/s^2
    public const float MarsSurfacePressure = 636f;   // Pa
    public const float MarsMeanTemp = -63f;          // Celsius
    public const float MarsSolDuration = 88775f;     // seconds (24h 37m 22s)
    public const float MarsSolarConstant = 589f;     // W/m^2 (at perihelion)

    // Atmosphere composition
    public const float CO2_Percentage = 95.32f;
    public const float N2_Percentage = 2.7f;
    public const float Ar_Percentage = 1.6f;

    // ═══════════════════════════════════════════════
    // VISUAL REFERENCE (from Opportunity photos)
    // ═══════════════════════════════════════════════

    // Sky colors (butterscotch, based on Opportunity panoramas)
    public static readonly Color SkyZenith = new Color(0.14f, 0.10f, 0.06f);
    public static readonly Color SkyHorizon = new Color(0.65f, 0.45f, 0.25f);
    public static readonly Color SkyGround = new Color(0.38f, 0.28f, 0.16f);

    // Terrain colors (Meridiani Planum: dark basaltic sand + hematite)
    public static readonly Color TerrainBase = new Color(0.45f, 0.35f, 0.25f);
    public static readonly Color TerrainDark = new Color(0.28f, 0.22f, 0.16f);
    public static readonly Color TerrainLight = new Color(0.62f, 0.50f, 0.38f);

    // Sun color on Mars (filtered through CO2 atmosphere)
    public static readonly Color SunColor = new Color(1f, 0.78f, 0.52f);

    // Fog/haze color
    public static readonly Color DustHaze = new Color(0.65f, 0.48f, 0.30f);

    // ═══════════════════════════════════════════════
    // UTILITY METHODS
    // ═══════════════════════════════════════════════

    /// <summary>
    /// Convert real Mars lat/lon to Unity world position.
    /// Center of AOI maps to Unity origin (0, 0, 0).
    /// </summary>
    public static Vector3 LatLonToUnity(double lat, double lonE)
    {
        double centerLat = (AOI_LatMin + AOI_LatMax) / 2.0;
        double centerLon = (AOI_LonMin + AOI_LonMax) / 2.0;

        float x = (float)((lonE - centerLon) * MetersPerDegree);
        float z = (float)((lat - centerLat) * MetersPerDegree);

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// Get Eagle Crater position in Unity coordinates.
    /// </summary>
    public static Vector3 GetEagleCraterPosition()
    {
        return LatLonToUnity(LandingLat, LandingLonE);
    }

    /// <summary>
    /// Get a time-of-day factor (0-1) based on simulated sol time.
    /// </summary>
    public static float GetDayFactor(float solTimeSeconds)
    {
        return Mathf.Max(0, Mathf.Sin(solTimeSeconds / MarsSolDuration * Mathf.PI));
    }
}
