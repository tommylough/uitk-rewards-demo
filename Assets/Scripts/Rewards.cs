using UnityEngine;
using UnityEngine.UIElements;

public class Rewards : VisualElement
{
    VisualElement buttonsContainer;

    JuicyReward rewardContainer;
    
    RewardButton rewardButton;
    
    RewardsDataSO dataSo;

    bool isPlaying;

    public Rewards()
    {
        dataSo = Resources.Load<RewardsDataSO>("ScriptableObjects/RewardsData");
    
        Generate();

        EnableEvents();
    }
    
    public void DisableEvents()
    {
        Signals.OnParticlesComplete -= () => SetDisableState(false);
        rewardButton.OnButtonTouched -= OnRewardButtonTouched;
        
        rewardContainer.DisableEvents();
    }

    void EnableEvents()
    {
        //DisableEvents();
        
        rewardButton.OnButtonTouched += OnRewardButtonTouched;
        
        Signals.OnParticlesComplete += EnableButtonAfterDelay;
    }
    
    // Janky way to deal with particle system complete event timing
    void EnableButtonAfterDelay()
    {
        schedule.Execute(() => SetDisableState(false)).StartingIn(1000);
    }

    void Generate()
    {
        AddToClassList("rewards-container");

        Add(GenerateRewardContainer());
        Add(GenerateButton());
    }

    VisualElement GenerateRewardContainer()
    {
        rewardContainer = new JuicyReward(dataSo.juicyRewardsData);

        return rewardContainer;
    }

    VisualElement GenerateButton()
    {
        buttonsContainer = UIToolkitUtils.Create("buttons-container");

        rewardButton = new RewardButton(0, SpriteSliceManager.GetSpriteSliceByName("GUI_52"));

        buttonsContainer.Add(rewardButton);

        return buttonsContainer;
    }

    void SetDisableState(bool dim)
    {
        if (dim)
        {
            buttonsContainer.AddToClassList("disableButton");
            buttonsContainer.pickingMode = PickingMode.Ignore;
            isPlaying = true;
        }
        else
        {
            buttonsContainer.RemoveFromClassList("disableButton");
            buttonsContainer.pickingMode = PickingMode.Position;
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