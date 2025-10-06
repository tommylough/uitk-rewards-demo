using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    [SerializeField] List<Sprite> sprits;
    [SerializeField] UIParticleEffect particleEffect;
    
    Rewards rewards;
    VisualElement particleContainer;
    
    void Start()
    {
        StartCoroutine(Generate());
    }

    void OnEnable()
    {
        Signals.OnSpinComplete += StartEffects;
    }
    
    void OnDisable()
    {
        Signals.OnSpinComplete -= StartEffects;
        
        rewards?.DisableEvents();
    }

    void OnValidate()
    {
        if (Application.isPlaying || !gameObject.activeInHierarchy) return;

        StartCoroutine(Generate());
    }
    
    IEnumerator Generate()
    {
        yield return null;
        
        SpriteSliceManager.Initialize(sprits);
        
        VisualElement root = uiDocument.rootVisualElement;
        
        root.Clear();
        StyleSheet styleSheet = Resources.Load<StyleSheet>("UI/Styles/Rewards");
        root.styleSheets.Add(styleSheet);

        root.Add(GenerateUI());
        root.Add(GenerateParticleContainer()); // Add to root after main UI
        
        // Setup particle effects after UI is created
        SetupParticleEffects();
    }
    
    void GenerateTempImage()
    {
        VisualElement tempImage = UIToolkitUtils.Create<Image>("temp-bg-image");
        uiDocument.rootVisualElement.Add(tempImage);
    }

    VisualElement GenerateUI()
    {
        var mobileContainer = UIToolkitUtils.Create("mobile-container");
        
        rewards = new Rewards();
        
        mobileContainer.Add(rewards);
        
        return mobileContainer;
    }
    
    VisualElement GenerateParticleContainer()
    {
        particleContainer = UIToolkitUtils.Create("particle-container");
        particleContainer.pickingMode = PickingMode.Ignore;
        return particleContainer;
    }
    
    void SetupParticleEffects()
    {
        if (particleEffect != null && particleContainer != null)
        {
            // Ensure container has NO background
            particleContainer.style.backgroundImage = StyleKeyword.None;
        
            particleEffect.SetTargetElement(particleContainer);
        }
    }
    
    void StartEffects()
    {
        if (particleEffect != null)
        {
            particleEffect.PlayEffect();
        }
    }
}