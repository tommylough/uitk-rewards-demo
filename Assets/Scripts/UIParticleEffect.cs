using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class UIParticleEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ParticleRenderTexture particleRenderer;
    [SerializeField] ParticleData[] particleData;
    
    [Header("UI Binding")]
    public VisualElement targetElement;

    int effectCount;
    
    void Start()
    {
        SetupParticleSystems();
    }
    
    public void SetTargetElement(VisualElement element)
    {
        targetElement = element;
        
        // Clear any background-image that might be set
        targetElement.style.backgroundImage = StyleKeyword.None;
        
        CreateParticleOverlay();
    }
    
    void CreateParticleOverlay()
    {
        if (targetElement == null || particleRenderer.renderTexture == null) return;
        
        // Remove any existing overlay
        var existingOverlay = targetElement.Q("particle-overlay");
        if (existingOverlay != null)
            targetElement.Remove(existingOverlay);
        
        // Create overlay element that renders on top
        var particleOverlay = new VisualElement();
        particleOverlay.name = "particle-overlay";
        particleOverlay.style.position = Position.Absolute;
        particleOverlay.style.width = Length.Percent(100);
        particleOverlay.style.height = Length.Percent(100);
        particleOverlay.style.left = 0;
        particleOverlay.style.top = 0;
        particleOverlay.style.backgroundImage = Background.FromRenderTexture(particleRenderer.renderTexture);
        particleOverlay.pickingMode = PickingMode.Ignore;
        
        // Add as last child (renders on top)
        targetElement.Add(particleOverlay);
    }
    
    public void Initialize()
    {
        // Only use overlay approach, don't set background-image
        CreateParticleOverlay();
    }
    
    void SetupParticleSystems()
    {
        foreach (var data in particleData)
        {
            data.ps.gameObject.layer = GetLayerFromMask(particleRenderer.particleLayer);
        }
    }
    
    public void PlayEffect(string effectName = "")
    {
        effectCount = 0;
        
        foreach (var data in particleData)
        {
            if (string.IsNullOrEmpty(effectName) || data.ps.name.Contains(effectName))
            {
                StartCoroutine(PlayEffectWithDelay(data.ps, data.soundName, data.delay));
            }
        }
    }
    
    IEnumerator PlayEffectWithDelay(ParticleSystem ps, string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
    
        ps.Play();
        SoundManager.Instance.PlaySFX(soundName);

        if (effectCount == particleData.Length - 1)
        {
            Invoke(nameof(SendOnCompleteSignal), 4);
        }
        
        effectCount++;
    }
    
    void SendOnCompleteSignal()
    {
        Signals.OnComplete?.Invoke();
    }
    
    public void StopEffect()
    {
        foreach (var data in particleData)
        {
            data.ps.Stop();
        }
    }
    
    public void PositionCameraAtUIElement()
    {
        if (targetElement == null) 
        {
            Debug.LogWarning("Target element not set. Call SetTargetElement() first.");
            return;
        }
        
        // Get UI element world position
        Rect elementRect = targetElement.worldBound;
        Vector3 screenCenter = new Vector3(elementRect.center.x, Screen.height - elementRect.center.y, 0);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenCenter);
        worldPos.z = particleRenderer.particleCamera.transform.position.z;
        
        particleRenderer.SetCameraPosition(worldPos);
        
        // Scale camera to match UI element size (made bigger)
        float uiSize = Mathf.Max(elementRect.width, elementRect.height) * 0.05f; // Changed from 0.01f to 0.05f for 5x bigger
        particleRenderer.SetCameraSize(uiSize);
    }
    
    int GetLayerFromMask(LayerMask mask)
    {
        int layer = 0;
        int maskValue = mask.value;
        while (maskValue > 1)
        {
            maskValue >>= 1;
            layer++;
        }
        return layer;
    }
}