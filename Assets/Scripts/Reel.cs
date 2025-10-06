using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Reel : VisualElement
{
    VisualElement imageStrip;
    VisualElement sparkle;
    VisualElement flash;

    ReelDataSO data;

    float speedMultiplierCount;
    float imageStripTop;
    float imageStripHeight;
    float viewportCenterY;

    int sparkleTransitionCount;
    int flashTransitionCount;

    int spinLoopCount;

    List<float> reelImagePositions = new();

    List<string> stripSliceNames = new()
    {
        "GUI_22",
        "GUI_20",
        "GUI_21",
        "GUI_26"
    };

    IVisualElementScheduledItem spinSchedule;
    IVisualElementScheduledItem effectSchedule;

    public Reel(ReelDataSO reelData)
    {
        data = reelData;
        
        InitData();

        AddToClassList("reel-container");

        Add(GenerateWheel());
        Add(GenerateCover());

        sparkle = GenerateSparkle();
        flash = GenerateFlash();

        Add(sparkle);
        Add(flash);

        EnableEvents();
    }

    void InitData()
    {
        UIToolkitUtils.DuplicateListItems(stripSliceNames);
    }

    public void DisableEvents()
    {
        Signals.OnPanelShowComplete -= StartSpin;
    }

    void EnableEvents()
    {
        DisableEvents();

        Signals.OnPanelShowComplete += StartSpin;
        Signals.OnReset += ResetEffects;
    }

    void StartSpin()
    {
        var topPosition = reelImagePositions[2];
        var bottomPosition = reelImagePositions[^2];
        var randomIndex = Random.Range(1, reelImagePositions.Count - 3);
        var targetPosition = reelImagePositions[randomIndex];
        var shouldFindTarget = false;
        
        speedMultiplierCount = data.initialSpeedMultiplier;
        spinLoopCount = 0;
        
        Signals.OnSlotWheelTargetSelected?.Invoke(stripSliceNames[randomIndex]);
        Signals.OnReelSpinStart?.Invoke();

        spinSchedule = schedule.Execute(() =>
        {
            UpdateSpin();
            
            if (Mathf.Abs(imageStripTop) + viewportCenterY < topPosition)
            {
                imageStripTop = -bottomPosition + viewportCenterY;
                spinLoopCount++;
            }

            if (spinLoopCount >= data.totalSpinLoops && !shouldFindTarget)
            {
                shouldFindTarget = true;
            }

            if (shouldFindTarget && Mathf.Abs(imageStripTop) + viewportCenterY > targetPosition)
            {
                StopSpin(targetPosition);
            }
            
        }).Every(data.spinScheduleIntervalMs);

        schedule.Execute(PlayEffects).StartingIn(data.effectStartDelayMs);

        SoundManager.Instance.PlayLoopingSFX("Wheel");
    }

    void UpdateSpin()
    {
        imageStrip.style.top = imageStripTop;

        imageStripTop += data.baseSpeed * speedMultiplierCount;

        speedMultiplierCount += data.speedIncrement;
    }

    void StopSpin(float targetPosition)
    {
        spinSchedule.Pause();
        SoundManager.Instance.StopLoopingSFX(true, data.soundFadeDuration);

        imageStrip.DOTop(-targetPosition + viewportCenterY, data.stopSpinDuration)
            .SetEase(EasingMode.EaseOutBounce)
            .OnComplete(() => Signals.OnSpinComplete?.Invoke())
            .Play();
    }
    
    void PlayEffects()
    {
        USSMultiTransition.Create(sparkle, data.sparkleEffectDuration)
            .AddScale(1f)
            .AddOpacity(1f)
            .SetEase(EasingMode.EaseInElastic)
            .SetLoops(data.sparkleLoopCount, LoopType.Yoyo)
            .Play();

        USSMultiTransition.Create(flash, data.flashEffectDuration)
            .AddScale(1f)
            .AddOpacity(1f)
            .SetEase(EasingMode.EaseInElastic)
            .SetLoops(data.flashLoopCount, LoopType.Yoyo)
            .Play();
    }

    void ResetEffects()
    {
        SoundManager.Instance.StopLoopingSFX(true, data.soundFadeDuration);
    }

    VisualElement GenerateWheel()
    {
        VisualElement mask = UIToolkitUtils.Create("reel-mask");

        imageStrip = UIToolkitUtils.Create("reel-strip");

        imageStrip.RegisterCallback<GeometryChangedEvent>((_) =>
        {
            imageStrip.UnregisterCallback<GeometryChangedEvent>((_) => { });
            imageStripHeight = imageStrip.layout.height;
        });

        mask.Add(imageStrip);

        GenerateStripElements(imageStrip);

        return mask;
    }

    VisualElement GenerateCover()
    {
        VisualElement element = UIToolkitUtils.Create("reel-cover");

        element.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            element.UnregisterCallback<GeometryChangedEvent>((_) => { });
            
            viewportCenterY = element.layout.height / 2;
            
            SetInitialWheelPositions();
        });

        return element;
    }

    void SetInitialWheelPositions()
    {
        if (reelImagePositions.Count == 0) return;

        imageStripTop = viewportCenterY - imageStripHeight / 2;
        imageStrip.style.top = imageStripTop;
    }

    VisualElement GenerateSparkle()
    {
        VisualElement element = UIToolkitUtils.Create("flash-sparkle");

        return element;
    }

    VisualElement GenerateFlash()
    {
        VisualElement element = UIToolkitUtils.Create("flash-cover");

        return element;
    }

    void GenerateStripElements(VisualElement strip)
    {
        for (var i = 0; i < stripSliceNames.Count; i++)
        {
            string sliceName = stripSliceNames[i];

            VisualElement stripElement = GenerateStripElement(sliceName);
            strip.Add(stripElement);

            EventCallback<GeometryChangedEvent> callback = null;
            callback = (evt) =>
            {
                OnStripElementsGeometryChanged(evt);
                (evt.target as VisualElement)?.UnregisterCallback(callback);
            };

            stripElement.RegisterCallback(callback);
        }
    }

    void OnStripElementsGeometryChanged(GeometryChangedEvent evt)
    {
        VisualElement element = evt.target as VisualElement;

        if (element == null)
        {
            Debug.LogError("OnGeometryChanged element is null");
            return;
        }

        float yPos = evt.newRect.y + element.layout.height / 2;

        reelImagePositions.Add(yPos);
    }

    VisualElement GenerateStripElement(string sliceName)
    {
        VisualElement image = UIToolkitUtils.Create<Image>("reel-strip-image");

        image.style.backgroundImage = new StyleBackground(SpriteSliceManager.GetSpriteSliceByName(sliceName));

        return image;
    }
}