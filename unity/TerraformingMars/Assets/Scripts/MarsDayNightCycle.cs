using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// Simulates a realistic Martian day/night cycle (Sol).
/// Controls sun rotation, intensity, sky colors, and fog.
/// </summary>
public class MarsDayNightCycle : MonoBehaviour
{
    [Header("Time")]
    [Tooltip("Current time in seconds (Sol = 88775s)")]
    public float solTime = 21600f; // Start at ~6:00 (quarter sol)
    [Tooltip("Time multiplier (1 = real time, 60 = 1 sol per ~25 min)")]
    public float timeScale = 60f;

    [Header("References")]
    public Light sunLight;
    public Volume atmosphereVolume;

    [Header("Sun Settings")]
    public float maxLux = 80000f;
    public float minLux = 0f;

    private GradientSky gradientSky;
    private Fog fog;
    private ColorAdjustments colorAdj;
    private Exposure exposure;
    private HDAdditionalLightData hdSun;

    void Start()
    {
        // Find sun if not assigned
        if (sunLight == null)
        {
            GameObject sunObj = GameObject.Find("Mars Sun");
            if (sunObj != null) sunLight = sunObj.GetComponent<Light>();
        }

        // Find volume if not assigned
        if (atmosphereVolume == null)
        {
            GameObject volObj = GameObject.Find("Mars Atmosphere");
            if (volObj != null) atmosphereVolume = volObj.GetComponent<Volume>();
        }

        // Cache volume overrides
        if (atmosphereVolume != null && atmosphereVolume.profile != null)
        {
            atmosphereVolume.profile.TryGet(out gradientSky);
            atmosphereVolume.profile.TryGet(out fog);
            atmosphereVolume.profile.TryGet(out colorAdj);
            atmosphereVolume.profile.TryGet(out exposure);
        }

        if (sunLight != null)
        {
            hdSun = sunLight.GetComponent<HDAdditionalLightData>();
        }
    }

    void Update()
    {
        // Advance time
        solTime += Time.deltaTime * timeScale;
        if (solTime >= MarsRealData.MarsSolDuration)
            solTime -= MarsRealData.MarsSolDuration;

        float dayFactor = MarsRealData.GetDayFactor(solTime);
        float sunAngle = (solTime / MarsRealData.MarsSolDuration) * 360f - 90f;

        UpdateSun(dayFactor, sunAngle);
        UpdateAtmosphere(dayFactor);
    }

    void UpdateSun(float dayFactor, float sunAngle)
    {
        if (sunLight == null) return;

        // Sun rotation (East to West arc)
        sunLight.transform.rotation = Quaternion.Euler(
            sunAngle,  // Altitude
            -30f,      // Azimuth offset
            0
        );

        // Intensity based on time of day
        float intensity = Mathf.Lerp(minLux, maxLux, dayFactor);

        if (hdSun != null)
        {
            hdSun.SetIntensity(intensity, LightUnit.Lux);
        }

        // Sun color shift (more orange at sunrise/sunset)
        float horizonFactor = 1f - Mathf.Abs(dayFactor - 0.5f) * 2f; // 0 at noon/midnight, 1 at horizons
        sunLight.color = Color.Lerp(
            MarsRealData.SunColor,
            new Color(1f, 0.5f, 0.2f), // Deep orange at horizon
            horizonFactor * 0.6f
        );
    }

    void UpdateAtmosphere(float dayFactor)
    {
        if (gradientSky != null)
        {
            // Sky gets darker at night
            Color topDay = MarsRealData.SkyZenith;
            Color topNight = new Color(0.01f, 0.01f, 0.02f);
            gradientSky.top.Override(Color.Lerp(topNight, topDay, dayFactor));

            Color midDay = MarsRealData.SkyHorizon;
            Color midNight = new Color(0.03f, 0.02f, 0.01f);
            gradientSky.middle.Override(Color.Lerp(midNight, midDay, dayFactor));

            Color botDay = MarsRealData.SkyGround;
            Color botNight = new Color(0.02f, 0.015f, 0.01f);
            gradientSky.bottom.Override(Color.Lerp(botNight, botDay, dayFactor));
        }

        if (fog != null)
        {
            // Fog thicker at dawn/dusk (dust storms)
            float fogDensity = Mathf.Lerp(400f, 1200f, dayFactor);
            fog.meanFreePath.Override(fogDensity);
        }

        if (colorAdj != null)
        {
            // Exposure shifts with daylight
            float exp = Mathf.Lerp(-1f, 0.5f, dayFactor);
            colorAdj.postExposure.Override(exp);
        }

        if (exposure != null)
        {
            float fixedExp = Mathf.Lerp(12f, 10f, dayFactor);
            exposure.fixedExposure.Override(fixedExp);
        }
    }

    /// <summary>
    /// Get formatted time string (HH:MM format, Mars sol time)
    /// </summary>
    public string GetTimeString()
    {
        float hours = solTime / 3600f;
        int h = Mathf.FloorToInt(hours);
        int m = Mathf.FloorToInt((hours - h) * 60f);
        return $"{h:00}:{m:00}";
    }

    /// <summary>
    /// Get current temperature estimate based on time of day
    /// </summary>
    public float GetTemperature()
    {
        float dayFactor = MarsRealData.GetDayFactor(solTime);
        return -80f + dayFactor * 40f + Mathf.Sin(solTime * 0.001f) * 5f;
    }
}
