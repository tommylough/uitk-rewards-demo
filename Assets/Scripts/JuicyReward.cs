using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JuicyReward : VisualElement
{
    VisualElement rewardContainer;
    VisualElement rewardPanel;
    VisualElement topRibbon;

    CurvedText curvedText;

    RouletteWheel rouletteWheel;
    
    ShakeEffect shakeEffect;
    

    public JuicyReward()
    {
        Generate();
        
        EnableEvents();
    }
    
    void EnableEvents()
    {
        Signals.OnStartAnimation -= Play;
        Signals.OnComplete -= Reset;
        
        Signals.OnStartAnimation += Play;
        Signals.OnComplete += Reset;
    }
    
    void Reset() {
        topRibbon.RemoveFromClassList("show-reward-top-ribbon");
        rewardPanel.RemoveFromClassList("show-reward-panel");
        
        ResetTextAnimation();
    }

    void Play()
    {
        schedule.Execute(ShowTopRibbon).StartingIn(250);
        schedule.Execute(ShowRibbonText).StartingIn(750);
        
        schedule.Execute(ShowPanel).StartingIn(2000);
        schedule.Execute(StartShake).StartingIn(3000);
    }

    void ShowTopRibbon()
    {
        topRibbon.AddToClassList("show-reward-top-ribbon");
        SoundManager.Instance.PlaySFX("ShowRibbon");
    }

    void ShowPanel()
    {
        rewardPanel.AddToClassList("show-reward-panel");
        SoundManager.Instance.PlaySFX("ShowPanel");
        
        Signals.OnSpinStart?.Invoke();
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
        VisualElement mask = UIToolkitUtils.Create("reward-container-panel-mask");
        
        rewardPanel = UIToolkitUtils.Create("reward-container-panel");
        rewardPanel.style.backgroundImage = new StyleBackground(SpriteSliceManager.GetSpriteSliceByName("GUI_28"));
        
        rewardPanel.Add(GenerateRoulette());

        mask.Add(rewardPanel);
        
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
        
        foreach (var label in characterLabels)
        {
            label.style.transitionDelay = new List<TimeValue> { new (delay) };
            label.style.opacity = 1;
            label.style.scale = new Scale(new Vector2(1, 1));
            
            delay += 0.05f;
        }
    }
    
    void ResetTextAnimation()
    {
        List<Label> characterLabels = curvedText.GetLabels();

        foreach (var label in characterLabels)
        {
            label.style.transitionDuration = new List<TimeValue> { new (0.1f) };
            label.style.transitionDelay = new List<TimeValue> { new (0) };
            label.style.opacity = 0;
            label.style.scale = new Scale(new Vector2(5, 5));
        }
    }

    VisualElement GenerateRoulette()
    {
        rouletteWheel = new RouletteWheel();

        return rouletteWheel;
    }
}