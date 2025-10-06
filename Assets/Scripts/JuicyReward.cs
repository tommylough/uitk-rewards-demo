using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JuicyReward : VisualElement
{
    VisualElement rewardContainer;
    VisualElement rewardPanel;
    VisualElement topRibbon;
    VisualElement mask;

    CurvedText curvedText;
    Reel reel;
    ShakeEffect shakeEffect;
    
    JuicyRewardsDataSO data;
    
    int ribbonTextTotal;
    int ribbonTextCount;

    float panelStartTop;

    public JuicyReward(JuicyRewardsDataSO dataSo)
    {
        data = dataSo;
        
        Generate();
        
        EnableEvents();
    }
    
    public void DisableEvents()
    {
        Signals.OnStartAnimation -= Play;
        Signals.OnParticlesComplete -= Reset;
        
        shakeEffect.DisableEvents();
    }
    
    void EnableEvents()
    {
        Signals.OnStartAnimation += Play;
        Signals.OnParticlesComplete += Reset;
        Signals.OnReelSpinStart += StartShake;
    }
    
    void Reset() {
        
        //HideTopRibbon();
        HidePanel();
    }

    void Play()
    {
        ShowTopRibbon();
    }
    
    void ShowTopRibbon()
    {
        USSMultiTransition.Create(topRibbon, data.showRibbonDuration)
            .AddScale(1f)
            .SetEase(EasingMode.EaseOutBack)
            .OnComplete(ShowRibbonText)
            .Play();
            
        SoundManager.Instance.PlaySFX("ShowRibbon");
    }

    void HideTopRibbon()
    {
        USSMultiTransition.Create(topRibbon, data.showRibbonDuration)
            .AddScale(0)
            .SetEase(EasingMode.EaseInBack)
            .OnComplete(ResetTextAnimation)
            .Play();
    }

    void ShowPanel()
    {
        float parentHeight = mask.resolvedStyle.height;
        float targetInPixels = parentHeight * data.showPanelTopTargetPercent;
        
        rewardPanel.DOTop(targetInPixels, data.showPanelTopDuration)
            .SetEase(EasingMode.EaseOutBounce)
            .OnComplete(() => Signals.OnPanelShowComplete?.Invoke())
            .Play();
        
        SoundManager.Instance.PlaySFX("ShowPanel");
    }
    
    void HidePanel()
    {
        rewardPanel.DOTop(panelStartTop, data.showPanelTopDuration)
            .SetEase(EasingMode.EaseInBack)
            .OnComplete(HideTopRibbon)
            .Play();
    }

    void StartShake()
    {
        shakeEffect.TriggerShake();
    }

    void Generate()
    {
        AddToClassList("rewards-container");

        Add(GenerateRewardContainer());
    }
    
    VisualElement GenerateRewardContainer()
    {
        rewardContainer = UIToolkitUtils.Create("reward-container");

        rewardContainer.Add(GeneratePanel());
        rewardContainer.Add(GenerateTopRibbon());
        
        shakeEffect = new ShakeEffect(rewardContainer);
        
        return rewardContainer;
    }

    VisualElement GeneratePanel()
    {
        mask = UIToolkitUtils.Create("reward-container-panel-mask");
        
        rewardPanel = UIToolkitUtils.Create("reward-container-panel");
        rewardPanel.style.backgroundImage = new StyleBackground(SpriteSliceManager.GetSpriteSliceByName("GUI_28"));
        
        rewardPanel.Add(Generatereel());

        mask.Add(rewardPanel);
        
        EventCallback<GeometryChangedEvent> callback = null;
        callback = (_) =>
        {
            rewardPanel.UnregisterCallback(callback);
            panelStartTop = rewardPanel.resolvedStyle.top;
        };
        rewardPanel.RegisterCallback(callback);
        
        return mask;
    }

    VisualElement GenerateTopRibbon()
    {
        topRibbon = UIToolkitUtils.Create("reward-top-ribbon");
        topRibbon.style.backgroundImage = new StyleBackground(SpriteSliceManager.GetSpriteSliceByName("GUI_59")); ;
        
        VisualElement curvedTextContainer = UIToolkitUtils.Create("curved-text-container");
        
        curvedText = new CurvedText("REWARDS", 500f, 0.1f)
        {
            CharacterSpacing = 260f
        };
        
        curvedText.SetFontSize(40);
        curvedText.SetTextColor(Color.white);

        curvedTextContainer.Add(curvedText);    
        
        topRibbon.Add(curvedTextContainer);
        
        return topRibbon;
    }
    
    void ShowRibbonText()
    {
        SoundManager.Instance.PlaySFX("ShowText");
        
        List<Label> characterLabels = curvedText.GetLabels();

        float delay = 0f;
        
        ribbonTextTotal = characterLabels.Count;
        
        foreach (var label in characterLabels)
        {
            USSMultiTransition.Create(label, data.ribbonTextDuration)
                .AddScale(data.ribbonTextScale)
                .AddOpacity(1f)
                .SetEase(EasingMode.EaseOutBack)
                .SetDelay(delay)
                .OnComplete(ShowRibbonTextComplete)
                .Play();
            
            delay += data.ribbonTextStaggerIncrement;
        }
    }
    
    void ShowRibbonTextComplete()
    {
        ribbonTextCount++;
        
        if(ribbonTextCount != ribbonTextTotal -1) 
            return;
        
        ShowPanel();
    }
    
    void ResetTextAnimation()
    {
        ribbonTextCount = 0;
        
        List<Label> characterLabels = curvedText.GetLabels();

        foreach (var label in characterLabels)
        {
            label.style.transitionDuration = new List<TimeValue> { new (0.1f) };
            label.style.transitionDelay = new List<TimeValue> { new (0) };
            label.style.opacity = 0;
            label.style.scale = new Scale(new Vector2(5, 5));
        }
    }

    VisualElement Generatereel()
    {
        reel = new Reel(data.reelData);

        return reel;
    }
}