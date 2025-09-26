using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RouletteWheel : VisualElement
{
    VisualElement imageStrip;
    VisualElement sparkle;
    VisualElement flash;

    float stripPosition;
    float loopPosition;
    float speedMultiplierCount = 1;
    float speedMultiplier = 0.01f;

    int sparkleTransitionCount;
    int flashTransitionCount;
    int effectCount = 0;

    string[] stripSliceNames =
    {
        "GUI_22",
        "GUI_20",
        "GUI_21",
        "GUI_26"
    };

    IVisualElementScheduledItem spinSchedule;
    IVisualElementScheduledItem effectSchedule;

    public RouletteWheel()
    {
        AddToClassList("roulette-container");

        Add(GenerateWheel());
        Add(GenerateCover());

        sparkle = GenerateSparkle();
        flash = GenerateFlash();

        Add(sparkle);
        Add(flash);

        EnableEvents();
    }

    void EnableEvents()
    {
        Signals.OnSpinStart -= StartSpin;
        Signals.OnStartSparkles -= PlayEffects;
        
        Signals.OnSpinStart += StartSpin;
        Signals.OnStartSparkles += PlayEffects;
    }

    void StartSpin()
    {
        spinSchedule = schedule.Execute(() =>
        {
            imageStrip.style.top = stripPosition;

            stripPosition -= 2 * speedMultiplierCount;

            speedMultiplierCount += speedMultiplier;

            if (stripPosition <= loopPosition - imageStrip.layout.height / 2)
            {
                stripPosition = loopPosition;
            }
        }).Every(10);

        schedule.Execute(PlayEffects).StartingIn(1000);

        schedule.Execute(StopEffects).StartingIn(4000);

        SoundManager.Instance.PlayLoopingSFX("Wheel");
    }

    void PlayEffects()
    {
        effectSchedule = schedule.Execute(() =>
        {
            sparkleTransitionCount = 0;
            flashTransitionCount = 0;

            sparkle.style.opacity = 1;
            sparkle.style.scale = new StyleScale(new Vector2(1, 1));

            flash.style.opacity = 0.9f;
            flash.style.scale = new StyleScale(new Vector2(1, 1));
        }).Every(1000);
    }

    void StopEffects()
    {
        spinSchedule.Pause();
        effectSchedule.Pause();
        SoundManager.Instance.StopLoopingSFX(true, 0.5f);
        Signals.OnSpinTimedOut?.Invoke();
        schedule.Execute(() => { Signals.OnReset?.Invoke(); }).StartingIn(4000);

        imageStrip.style.top = imageStrip.style.top.value.value + 40;
    }

    VisualElement GenerateWheel()
    {
        VisualElement mask = UIToolkitUtils.Create("roulette-mask");

        imageStrip = UIToolkitUtils.Create("roulette-strip");

        mask.Add(imageStrip);

        GenerateStripElements(imageStrip);
        GenerateStripElements(imageStrip);

        return mask;
    }

    VisualElement GenerateCover()
    {
        VisualElement element = UIToolkitUtils.Create("roulette-cover");

        return element;
    }

    VisualElement GenerateSparkle()
    {
        VisualElement element = UIToolkitUtils.Create("flash-sparkle");

        element.RegisterCallback<TransitionEndEvent>(OnSparkleEnded);

        return element;
    }

    void OnSparkleEnded(TransitionEndEvent evt)
    {
        if (evt.AffectsProperty(new StylePropertyName("scale")))
        {
            if (sparkleTransitionCount == 0)
            {
                sparkle.style.opacity = 0;
                sparkle.style.scale = new StyleScale(new Vector2(0, 0));
                return;
            }

            sparkleTransitionCount++;
        }
    }

    VisualElement GenerateFlash()
    {
        VisualElement element = UIToolkitUtils.Create("flash-cover");

        element.RegisterCallback<TransitionEndEvent>(OnFlashEnded);

        return element;
    }

    void OnFlashEnded(TransitionEndEvent evt)
    {
        if (evt.AffectsProperty(new StylePropertyName("scale")))
        {
            if (flashTransitionCount == 0)
            {
                flash.style.opacity = 0;
                flash.style.scale = new StyleScale(new Vector2(0, 0));
                return;
            }

            flashTransitionCount++;
        }
    }

    void GenerateStripElements(VisualElement strip)
    {
        foreach (var sliceName in stripSliceNames)
        {
            VisualElement stripElement = GenerateStripElement(sliceName);
            strip.Add(stripElement);
        }
    }

    VisualElement GenerateStripElement(string sliceName)
    {
        VisualElement image = UIToolkitUtils.Create<Image>("roulette-strip-image");

        image.style.backgroundImage = new StyleBackground(SpriteSliceManager.GetSpriteSliceByName(sliceName));

        return image;
    }
}