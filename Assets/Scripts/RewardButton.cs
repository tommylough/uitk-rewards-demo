using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardButton: VisualElement
{
    public Action <int> OnButtonTouched;

    readonly int index;
        
    public RewardButton(int index, Sprite sprite)
    {
        this.index = index;
        
        AddToClassList("reward-button");
        
        name = $"reward-button-{index}";
        
        style.backgroundImage = new StyleBackground(sprite);
        
        Label label = new("<b>REWARD ME!</b>");
        label.AddToClassList("reward-button-label");
        Add(label);

        EnableEvents();
    }

    void EnableEvents()
    {
        UnregisterCallback<PointerUpEvent>(_ =>
        {
            OnButtonTouched?.Invoke(index);
        });
        
        RegisterCallback<PointerUpEvent>(_ =>
        {
            OnButtonTouched?.Invoke(index);
        });
    }
}