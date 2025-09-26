using System;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class Rewards : VisualElement
{
    VisualElement buttonsContainer;

    JuicyReward rewardContainer;

    bool isPlaying;

    public Rewards()
    {
        Generate();

        EnableEvents();
    }

    void EnableEvents()
    {
        Signals.OnComplete -= () => SetDisableState(false);
        
        Signals.OnComplete += () => SetDisableState(false);
    }

    void Generate()
    {
        AddToClassList("rewards-container");

        Add(GenerateRewardContainer());
        Add(GenerateButton());
    }

    VisualElement GenerateRewardContainer()
    {
        rewardContainer = new JuicyReward();

        return rewardContainer;
    }

    VisualElement GenerateButton()
    {
        buttonsContainer = UIToolkitUtils.Create("buttons-container");

        var button = new RewardButton(0, SpriteSliceManager.GetSpriteSliceByName("GUI_52"));

        button.OnButtonTouched += OnRewardButtonTouched;

        buttonsContainer.Add(button);

        return buttonsContainer;
    }

    void SetDisableState(bool dim)
    {
        if (dim)
        {
            buttonsContainer.AddToClassList("disableButton");
            isPlaying = true;
        }
        else
        {
            buttonsContainer.RemoveFromClassList("disableButton");
            isPlaying = false;
        }
    }

    void OnRewardButtonTouched(int index)
    {
        if (isPlaying) return;

        isPlaying = true;

        SetDisableState(true);

        Signals.OnStartAnimation?.Invoke();
    }
}