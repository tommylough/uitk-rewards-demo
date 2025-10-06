using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class USSMultiTransition
{
    struct PropertyTransition
    {
        public string propertyName;
        public Action<VisualElement> applyStartValue;
        public Action<VisualElement> applyEndValue;
    }

    VisualElement target;
    List<PropertyTransition> properties = new List<PropertyTransition>();
    float duration;
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
    HashSet<string> completedProperties = new HashSet<string>();

    USSMultiTransition(VisualElement element, float dur)
    {
        target = element;
        duration = dur;
    }

    public static USSMultiTransition Create(VisualElement element, float duration)
    {
        return new USSMultiTransition(element, duration);
    }

    public USSMultiTransition AddOpacity(float targetValue)
    {
        float startValue = target.resolvedStyle.opacity;
        properties.Add(new PropertyTransition
        {
            propertyName = "opacity",
            applyStartValue = el => el.style.opacity = startValue,
            applyEndValue = el => el.style.opacity = targetValue
        });
        return this;
    }

    public USSMultiTransition AddScale(Vector2 targetValue)
    {
        Scale startValue = target.resolvedStyle.scale;
        properties.Add(new PropertyTransition
        {
            propertyName = "scale",
            applyStartValue = el => el.style.scale = startValue,
            applyEndValue = el => el.style.scale = new Scale(new Vector3(targetValue.x, targetValue.y, 1f))
        });
        return this;
    }

    public USSMultiTransition AddScale(float targetValue)
    {
        return AddScale(new Vector2(targetValue, targetValue));
    }

    public USSMultiTransition AddRotate(float degrees)
    {
        Rotate startValue = target.resolvedStyle.rotate;
        properties.Add(new PropertyTransition
        {
            propertyName = "rotate",
            applyStartValue = el => el.style.rotate = startValue,
            applyEndValue = el => el.style.rotate = new Rotate(new Angle(degrees, AngleUnit.Degree))
        });
        return this;
    }

    public USSMultiTransition AddTranslate(Vector2 targetValue)
    {
        Translate startValue = target.resolvedStyle.translate;
        properties.Add(new PropertyTransition
        {
            propertyName = "translate",
            applyStartValue = el => el.style.translate = startValue,
            applyEndValue = el => el.style.translate = new Translate(targetValue.x, targetValue.y)
        });
        return this;
    }

    public USSMultiTransition AddBackgroundColor(Color targetValue)
    {
        Color startValue = target.resolvedStyle.backgroundColor;
        properties.Add(new PropertyTransition
        {
            propertyName = "background-color",
            applyStartValue = el => el.style.backgroundColor = startValue,
            applyEndValue = el => el.style.backgroundColor = targetValue
        });
        return this;
    }

    public USSMultiTransition AddColor(Color targetValue)
    {
        Color startValue = target.resolvedStyle.color;
        properties.Add(new PropertyTransition
        {
            propertyName = "color",
            applyStartValue = el => el.style.color = startValue,
            applyEndValue = el => el.style.color = targetValue
        });
        return this;
    }

    public USSMultiTransition AddWidth(float targetValue)
    {
        float startValue = target.resolvedStyle.width;
        properties.Add(new PropertyTransition
        {
            propertyName = "width",
            applyStartValue = el => el.style.width = startValue,
            applyEndValue = el => el.style.width = targetValue
        });
        return this;
    }

    public USSMultiTransition AddHeight(float targetValue)
    {
        float startValue = target.resolvedStyle.height;
        properties.Add(new PropertyTransition
        {
            propertyName = "height",
            applyStartValue = el => el.style.height = startValue,
            applyEndValue = el => el.style.height = targetValue
        });
        return this;
    }

    public USSMultiTransition AddBorderRadius(float targetValue)
    {
        float startValue = target.resolvedStyle.borderBottomLeftRadius;
        properties.Add(new PropertyTransition
        {
            propertyName = "border-radius",
            applyStartValue = el => el.style.borderBottomLeftRadius = el.style.borderBottomRightRadius = el.style.borderTopLeftRadius = el.style.borderTopRightRadius = startValue,
            applyEndValue = el => el.style.borderBottomLeftRadius = el.style.borderBottomRightRadius = el.style.borderTopLeftRadius = el.style.borderTopRightRadius = targetValue
        });
        return this;
    }

    public USSMultiTransition SetDelay(float delaySeconds)
    {
        delay = delaySeconds;
        return this;
    }

    public USSMultiTransition SetEase(EasingMode mode)
    {
        easingMode = mode;
        return this;
    }

    public USSMultiTransition SetLoops(int loopCount, LoopType type = LoopType.Restart)
    {
        loops = loopCount;
        loopType = type;
        return this;
    }

    public USSMultiTransition OnComplete(Action callback)
    {
        onComplete = callback;
        return this;
    }

    public USSMultiTransition OnStart(Action callback)
    {
        onStart = callback;
        return this;
    }

    public USSMultiTransition OnUpdate(Action callback)
    {
        onUpdate = callback;
        return this;
    }

    public USSMultiTransition Play()
    {
        if (properties.Count == 0)
        {
            return this;
        }

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
        completedProperties.Clear();

        SetupTransitionProperties();

        transitionEndCallback = OnTransitionEnd;
        target.RegisterCallback(transitionEndCallback);

        onStart?.Invoke();

        foreach (var prop in properties)
        {
            var endAction = isYoyoReverse ? prop.applyStartValue : prop.applyEndValue;
            endAction(target);
        }

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
        List<StylePropertyName> propertyNames = new List<StylePropertyName>();
        List<TimeValue> durations = new List<TimeValue>();
        List<EasingFunction> timingFunctions = new List<EasingFunction>();
        List<TimeValue> delays = new List<TimeValue>();

        foreach (var prop in properties)
        {
            propertyNames.Add(new StylePropertyName(prop.propertyName));
            durations.Add(new TimeValue(duration, TimeUnit.Second));
            timingFunctions.Add(new EasingFunction(easingMode));
            delays.Add(new TimeValue(delay, TimeUnit.Second));
        }

        target.style.transitionProperty = propertyNames;
        target.style.transitionDuration = durations;
        target.style.transitionTimingFunction = timingFunctions;
        target.style.transitionDelay = delays;
    }

    void OnTransitionEnd(TransitionEndEvent evt)
    {
        foreach (var styleProp in evt.stylePropertyNames)
        {
            foreach (var prop in properties)
            {
                if (styleProp.Equals(new StylePropertyName(prop.propertyName)))
                {
                    completedProperties.Add(prop.propertyName);
                    break;
                }
            }
        }

        bool allCompleted = completedProperties.Count >= properties.Count;

        if (!allCompleted)
        {
            return;
        }

        completedProperties.Clear();
        currentLoop++;

        bool shouldContinue = (loops == -1) || (loops > 0 && currentLoop < loops);

        if (shouldContinue)
        {
            if (loopType == LoopType.Yoyo)
            {
                isYoyoReverse = !isYoyoReverse;
                foreach (var prop in properties)
                {
                    var nextValue = isYoyoReverse ? prop.applyStartValue : prop.applyEndValue;
                    nextValue(target);
                }
            }
            else
            {
                foreach (var prop in properties)
                {
                    prop.applyEndValue(target);
                }
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
        foreach (var prop in properties)
        {
            prop.applyEndValue(target);
        }
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