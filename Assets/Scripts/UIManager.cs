using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    [SerializeField] List<Sprite> sprits;
    [SerializeField] UIParticleEffect particleEffect; // Add this reference
    
    Rewards rewards;
    VisualElement particleContainer; // Store reference to particle container
    
    void Start()
    {
        StartCoroutine(Generate());
    }

    void OnEnable()
    {
        Signals.OnSpinTimedOut += StartEffects;
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

        VisualElement ui = GenerateUI();
        root.Add(ui);
        
        // Create particle container at root level (renders on top)
        particleContainer = new VisualElement();
        particleContainer.name = "particle-container";
        particleContainer.style.position = Position.Absolute;
        particleContainer.style.width = Length.Percent(100);
        particleContainer.style.height = Length.Percent(100);
        particleContainer.style.left = 0;
        particleContainer.style.top = 0;
        particleContainer.pickingMode = PickingMode.Ignore;
        
        root.Add(particleContainer); // Add to root after main UI
        
        // Setup particle effects after UI is created
        SetupParticleEffects();
    }

    VisualElement GenerateUI()
    {
        var mobileContainer = UIToolkitUtils.Create("mobile-container");
        
        rewards = new Rewards();
        
        mobileContainer.Add(rewards);
        
        return mobileContainer;
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