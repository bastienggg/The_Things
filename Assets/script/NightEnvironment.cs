using UnityEngine;

public class NightEnvironment : MonoBehaviour
{
    [Header("Night Settings")]
    public Color nightAmbientColor = new Color(0.01f, 0.01f, 0.02f); // Très sombre
    public Color nightFogColor = new Color(0.02f, 0.02f, 0.05f);
    
    [Header("Fog Settings")]
    public bool enableFog = true;
    public FogMode fogMode = FogMode.ExponentialSquared;
    public float fogDensity = 0.08f; // Plus dense
    public float fogStartDistance = 5f;
    public float fogEndDistance = 30f;
    
    [Header("Directional Light (Moon)")]
    public Light directionalLight;
    public Color moonlightColor = new Color(0.15f, 0.15f, 0.2f);
    public float moonlightIntensity = 0.05f; // Très faible
    
    void Start()
    {
        SetupNightEnvironment();
    }
    
    void SetupNightEnvironment()
    {
        // Configuration de l'ambiance nocturne
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = nightAmbientColor;
        
        // Configuration du brouillard
        RenderSettings.fog = enableFog;
        RenderSettings.fogColor = nightFogColor;
        RenderSettings.fogMode = fogMode;
        
        if (fogMode == FogMode.ExponentialSquared || fogMode == FogMode.Exponential)
        {
            RenderSettings.fogDensity = fogDensity;
        }
        else
        {
            RenderSettings.fogStartDistance = fogStartDistance;
            RenderSettings.fogEndDistance = fogEndDistance;
        }
        
        // Configuration de la lumière directionnelle (lune)
        if (directionalLight == null)
        {
            // Chercher la lumière directionnelle dans la scène
            directionalLight = FindObjectOfType<Light>();
        }
        
        if (directionalLight != null && directionalLight.type == LightType.Directional)
        {
            directionalLight.color = moonlightColor;
            directionalLight.intensity = moonlightIntensity;
            
            // Orienter la lumière comme une lune (de haut)
            directionalLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
        
        // Désactiver le skybox ou le rendre très sombre
        RenderSettings.skybox = null;
        if (Camera.main != null)
        {
            Camera.main.backgroundColor = new Color(0.0f, 0.0f, 0.0f); // Noir total
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }
    }
    
    // Méthodes pour ajuster dynamiquement
    public void SetFogDensity(float density)
    {
        fogDensity = density;
        RenderSettings.fogDensity = density;
    }
    
    public void ToggleFog()
    {
        enableFog = !enableFog;
        RenderSettings.fog = enableFog;
    }
}
