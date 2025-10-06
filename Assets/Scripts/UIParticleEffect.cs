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
    
    public void SetTargetElement(VisualElement element)
    {
        targetElement = element;
        
        targetElement.style.backgroundImage = StyleKeyword.None;
        
        CreateParticleOverlay();
    }

    void OnEnable()
    {
        Signals.OnSlotWheelTargetSelected += OnSlotWheelTargetSelected;
    }
    
    void OnDisable()
    {
        Signals.OnSlotWheelTargetSelected -= OnSlotWheelTargetSelected;
    }
    
    void Start()
        {
            SetupParticleSystems();
        }
    
    void OnSlotWheelTargetSelected(string targetName)
    {
        ParticleSystem ps = particleData[3].ps;
        
        var textureSheet = ps.textureSheetAnimation;

        var slice = SpriteSliceManager.GetSpriteSliceByName(targetName);
        
        if (slice == null) return;
        
        textureSheet.SetSprite(0, slice);
    }
    
    void CreateParticleOverlay()
    {
        if (targetElement == null || particleRenderer.renderTexture == null) return;
        
        // Remove any existing overlay
        var existingOverlay = targetElement.Q("particle-overlay");
        if (existingOverlay != null)
            targetElement.Remove(existingOverlay);
        
        targetElement.Add(GenerateParticleOverlay());
    }
    
    VisualElement GenerateParticleOverlay()
    {
        VisualElement element = UIToolkitUtils.Create("particle-container");
        element.style.backgroundImage = Background.FromRenderTexture(particleRenderer.renderTexture);
        element.pickingMode = PickingMode.Ignore;
        
        return element;
    }
    
    public void Initialize()
    {
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
        Signals.OnParticlesComplete?.Invoke();
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
        
        float uiSize = Mathf.Max(elementRect.width, elementRect.height) * 0.05f;
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