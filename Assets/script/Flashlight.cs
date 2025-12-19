using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlightLight;
    public bool isOn = true;
    
    [Header("Light Properties")]
    public float normalIntensity = 8f;
    public float normalRange = 25f;
    
    void Start()
    {
        if (flashlightLight == null)
        {
            flashlightLight = GetComponent<Light>();
        }
        
        if (flashlightLight != null)
        {
            flashlightLight.intensity = normalIntensity;
            flashlightLight.range = normalRange;
            flashlightLight.enabled = isOn;
        }
    }
    
    void Update()
    {
        // Allumer/Éteindre la lampe avec F ou T
        if (Keyboard.current != null)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame || Keyboard.current.tKey.wasPressedThisFrame)
            {
                ToggleFlashlight();
            }
        }
    }
    
    void ToggleFlashlight()
    {
        isOn = !isOn;
        flashlightLight.enabled = isOn;
    }
}
