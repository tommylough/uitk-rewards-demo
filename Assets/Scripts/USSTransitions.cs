using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class USSTransition
{
    VisualElement target;
    Action<VisualElement> applyStartValue;
    Action<VisualElement> applyEndValue;
    string propertyName;
    float duration = 0.3f;
    float delay = 0f;
    EasingMode easingMode = EasingMode.Ease;
    Action onComplete;
    Action onStart;
    Action onUpdate;
    IVisualElementScheduledItem updateScheduler;
    bool isPlaying;
    bool isPaused;
    float elapsedTime;
    float startTime;
    int loops = 0;
    int currentLoop = 0;
    LoopType loopType = LoopType.Restart;
    bool isYoyoReverse = false;
    EventCallback<TransitionEndEvent> transitionEndCallback;

    public float Duration => duration + delay;
    public VisualElement Target => target;

    USSTransition(VisualElement element, string prop, Action<VisualElement> startVal, Action<VisualElement> endVal, float dur)
    {
        target = element;
        propertyName = prop;
        applyStartValue = startVal;
        applyEndValue = endVal;
        duration = dur;
    }

    public static USSTransition Opacity(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.opacity;
        return new USSTransition(
            element, 
            "opacity", 
            el => el.style.opacity = startValue,
            el => el.style.opacity = targetValue, 
            duration
        );
    }

    public static USSTransition Scale(VisualElement element, Vector2 targetValue, float duration)
    {
        Scale startValue = element.resolvedStyle.scale;
        return new USSTransition(
            element, 
            "scale", 
            el => el.style.scale = startValue,
            el => el.style.scale = new Scale(new Vector3(targetValue.x, targetValue.y, 1f)), 
            duration
        );
    }

    public static USSTransition Rotate(VisualElement element, float degrees, float duration)
    {
        Rotate startValue = element.resolvedStyle.rotate;
        return new USSTransition(
            element, 
            "rotate",
            el => el.style.rotate = startValue,
            el => el.style.rotate = new Rotate(new Angle(degrees, AngleUnit.Degree)), 
            duration
        );
    }

    public static USSTransition TranslateX(VisualElement element, float targetValue, float duration)
    {
        Translate startValue = element.resolvedStyle.translate;
        return new USSTransition(
            element, 
            "translate",
            el => el.style.translate = startValue,
            el => el.style.translate = new Translate(targetValue, 0), 
            duration
        );
    }

    public static USSTransition TranslateY(VisualElement element, float targetValue, float duration)
    {
        Translate startValue = element.resolvedStyle.translate;
        return new USSTransition(
            element, 
            "translate",
            el => el.style.translate = startValue,
            el => el.style.translate = new Translate(0, targetValue), 
            duration
        );
    }

    public static USSTransition Translate(VisualElement element, Vector2 targetValue, float duration)
    {
        Translate startValue = element.resolvedStyle.translate;
        return new USSTransition(
            element, 
            "translate",
            el => el.style.translate = startValue,
            el => el.style.translate = new Translate(targetValue.x, targetValue.y), 
            duration
        );
    }

    public static USSTransition BackgroundColor(VisualElement element, Color targetValue, float duration)
    {
        Color startValue = element.resolvedStyle.backgroundColor;
        return new USSTransition(
            element, 
            "background-color",
            el => el.style.backgroundColor = startValue,
            el => el.style.backgroundColor = targetValue, 
            duration
        );
    }

    public static USSTransition Color(VisualElement element, Color targetValue, float duration)
    {
        Color startValue = element.resolvedStyle.color;
        return new USSTransition(
            element, 
            "color",
            el => el.style.color = startValue,
            el => el.style.color = targetValue, 
            duration
        );
    }

    public static USSTransition BorderColor(VisualElement element, Color targetValue, float duration)
    {
        Color startValue = element.resolvedStyle.borderBottomColor;
        return new USSTransition(
            element, 
            "border-color",
            el => el.style.borderBottomColor = el.style.borderTopColor = el.style.borderLeftColor = el.style.borderRightColor = startValue,
            el => el.style.borderBottomColor = el.style.borderTopColor = el.style.borderLeftColor = el.style.borderRightColor = targetValue, 
            duration
        );
    }

    public static USSTransition Width(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.width;
        return new USSTransition(
            element, 
            "width",
            el => el.style.width = startValue,
            el => el.style.width = targetValue, 
            duration
        );
    }

    public static USSTransition Height(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.height;
        return new USSTransition(
            element, 
            "height",
            el => el.style.height = startValue,
            el => el.style.height = targetValue, 
            duration
        );
    }

    public static USSTransition MarginLeft(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.marginLeft;
        return new USSTransition(element, "margin-left", el => el.style.marginLeft = startValue, el => el.style.marginLeft = targetValue, duration);
    }

    public static USSTransition MarginTop(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.marginTop;
        return new USSTransition(element, "margin-top", el => el.style.marginTop = startValue, el => el.style.marginTop = targetValue, duration);
    }

    public static USSTransition MarginRight(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.marginRight;
        return new USSTransition(element, "margin-right", el => el.style.marginRight = startValue, el => el.style.marginRight = targetValue, duration);
    }

    public static USSTransition MarginBottom(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.marginBottom;
        return new USSTransition(element, "margin-bottom", el => el.style.marginBottom = startValue, el => el.style.marginBottom = targetValue, duration);
    }

    public static USSTransition PaddingLeft(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.paddingLeft;
        return new USSTransition(element, "padding-left", el => el.style.paddingLeft = startValue, el => el.style.paddingLeft = targetValue, duration);
    }

    public static USSTransition PaddingTop(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.paddingTop;
        return new USSTransition(element, "padding-top", el => el.style.paddingTop = startValue, el => el.style.paddingTop = targetValue, duration);
    }

    public static USSTransition PaddingRight(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.paddingRight;
        return new USSTransition(element, "padding-right", el => el.style.paddingRight = startValue, el => el.style.paddingRight = targetValue, duration);
    }

    public static USSTransition PaddingBottom(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.paddingBottom;
        return new USSTransition(element, "padding-bottom", el => el.style.paddingBottom = startValue, el => el.style.paddingBottom = targetValue, duration);
    }

    public static USSTransition BorderWidth(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.borderBottomWidth;
        return new USSTransition(element, "border-width", el => el.style.borderBottomWidth = el.style.borderTopWidth = el.style.borderLeftWidth = el.style.borderRightWidth = startValue, el => el.style.borderBottomWidth = el.style.borderTopWidth = el.style.borderLeftWidth = el.style.borderRightWidth = targetValue, duration);
    }

    public static USSTransition BorderRadius(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.borderBottomLeftRadius;
        return new USSTransition(element, "border-radius", el => el.style.borderBottomLeftRadius = el.style.borderBottomRightRadius = el.style.borderTopLeftRadius = el.style.borderTopRightRadius = startValue, el => el.style.borderBottomLeftRadius = el.style.borderBottomRightRadius = el.style.borderTopLeftRadius = el.style.borderTopRightRadius = targetValue, duration);
    }

    public static USSTransition Top(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.top;
        return new USSTransition(element, "top", el => el.style.top = startValue, el => el.style.top = targetValue, duration);
    }

    public static USSTransition Bottom(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.bottom;
        return new USSTransition(element, "bottom", el => el.style.bottom = startValue, el => el.style.bottom = targetValue, duration);
    }

    public static USSTransition Left(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.left;
        return new USSTransition(element, "left", el => el.style.left = startValue, el => el.style.left = targetValue, duration);
    }

    public static USSTransition Right(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.right;
        return new USSTransition(element, "right", el => el.style.right = startValue, el => el.style.right = targetValue, duration);
    }

    public static USSTransition FlexGrow(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.flexGrow;
        return new USSTransition(element, "flex-grow", el => el.style.flexGrow = startValue, el => el.style.flexGrow = targetValue, duration);
    }

    public static USSTransition FlexShrink(VisualElement element, float targetValue, float duration)
    {
        float startValue = element.resolvedStyle.flexShrink;
        return new USSTransition(element, "flex-shrink", el => el.style.flexShrink = startValue, el => el.style.flexShrink = targetValue, duration);
    }

    public USSTransition SetFrom(Action<VisualElement> fromAction)
    {
        applyStartValue = fromAction;
        return this;
    }

    public USSTransition SetDelay(float delaySeconds)
    {
        delay = delaySeconds;
        return this;
    }

    public USSTransition SetEase(EasingMode mode)
    {
        easingMode = mode;
        return this;
    }

    public USSTransition SetLoops(int loopCount, LoopType type = LoopType.Restart)
    {
        loops = loopCount;
        loopType = type;
        return this;
    }

    public USSTransition OnComplete(Action callback)
    {
        onComplete = callback;
        return this;
    }

    public USSTransition OnStart(Action callback)
    {
        onStart = callback;
        return this;
    }

    public USSTransition OnUpdate(Action callback)
    {
        onUpdate = callback;
        return this;
    }

    public USSTransition Play()
    {
        if (isPlaying && !isPaused)
        {
            return this;
        }

        if (isPaused)
        {
            isPaused = false;
            return this;
        }

        if (transitionEndCallback != null)
        {
            target.UnregisterCallback(transitionEndCallback);
            transitionEndCallback = null;
        }

        isPlaying = true;
        currentLoop = 0;
        isYoyoReverse = false;

        SetupTransitionProperties();
        
        transitionEndCallback = OnTransitionEnd;
        target.RegisterCallback(transitionEndCallback);

        onStart?.Invoke();
        applyEndValue(target);
        
        startTime = Time.time;
        updateScheduler = target.schedule.Execute(() => {
            if (!isPaused)
            {
                elapsedTime = Time.time - startTime;
                onUpdate?.Invoke();
            }
        }).Every(16);

        return this;
    }

    void SetupTransitionProperties()
    {
        target.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName(propertyName) };
        target.style.transitionDuration = new List<TimeValue> { new TimeValue(duration, TimeUnit.Second) };
        target.style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(easingMode) };
        target.style.transitionDelay = new List<TimeValue> { new TimeValue(delay, TimeUnit.Second) };
    }

    void OnTransitionEnd(TransitionEndEvent evt)
    {
        if (!evt.stylePropertyNames.Contains(new StylePropertyName(propertyName)))
        {
            return;
        }

        currentLoop++;

        bool shouldContinue = (loops == -1) || (loops > 0 && currentLoop < loops);

        if (shouldContinue)
        {
            if (loopType == LoopType.Yoyo)
            {
                isYoyoReverse = !isYoyoReverse;
                Action<VisualElement> nextValue = isYoyoReverse ? applyStartValue : applyEndValue;
                nextValue(target);
            }
            else
            {
                applyEndValue(target);
            }
        }
        else
        {
            Finish();
        }
    }

    void Finish()
    {
        if (updateScheduler != null)
        {
            updateScheduler.Pause();
            updateScheduler = null;
        }

        if (transitionEndCallback != null)
        {
            target.UnregisterCallback(transitionEndCallback);
            transitionEndCallback = null;
        }

        isPlaying = false;
        CleanupTransitionProperties();
        onComplete?.Invoke();
    }

    public void Pause()
    {
        isPaused = true;
        if (updateScheduler != null)
        {
            updateScheduler.Pause();
        }
    }

    public void Resume()
    {
        if (isPaused)
        {
            isPaused = false;
            startTime = Time.time - elapsedTime;
            if (updateScheduler != null)
            {
                updateScheduler.Resume();
            }
        }
    }

    public void Kill()
    {
        if (updateScheduler != null)
        {
            updateScheduler.Pause();
            updateScheduler = null;
        }
        if (transitionEndCallback != null)
        {
            target.UnregisterCallback(transitionEndCallback);
            transitionEndCallback = null;
        }
        isPlaying = false;
        isPaused = false;
        CleanupTransitionProperties();
    }

    public void Complete()
    {
        Kill();
        applyEndValue(target);
        onComplete?.Invoke();
    }

    void CleanupTransitionProperties()
    {
        if (target != null)
        {
            target.style.transitionProperty = new List<StylePropertyName>();
            target.style.transitionDuration = new List<TimeValue>();
            target.style.transitionTimingFunction = new List<EasingFunction>();
            target.style.transitionDelay = new List<TimeValue>();
        }
    }
}