using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class USSSequence
{
    struct SequenceItem
    {
        public float startTime;
        public USSTransition transition;
    }

    struct SequenceCallback
    {
        public float time;
        public Action callback;
    }

    List<SequenceItem> items = new List<SequenceItem>();
    List<SequenceCallback> callbacks = new List<SequenceCallback>();
    float currentDuration = 0f;
    float totalDuration = 0f;
    int loops = 0;
    int currentLoop = 0;
    LoopType loopType = LoopType.Restart;
    bool isPlaying = false;
    bool isPaused = false;
    float elapsedTime = 0f;
    Action onComplete;
    Action onStart;
    Action onUpdate;
    IVisualElementScheduledItem scheduledItem;
    float startTime;
    List<USSTransition> activeTransitions = new List<USSTransition>();
    HashSet<int> startedItems = new HashSet<int>();
    HashSet<int> executedCallbacks = new HashSet<int>();

    public static USSSequence Create()
    {
        return new USSSequence();
    }

    public USSSequence Append(USSTransition transition)
    {
        items.Add(new SequenceItem
        {
            startTime = currentDuration,
            transition = transition
        });
        currentDuration += transition.Duration;
        totalDuration = Mathf.Max(totalDuration, currentDuration);
        return this;
    }

    public USSSequence Join(USSTransition transition)
    {
        float lastStartTime = items.Count > 0 ? items[items.Count - 1].startTime : 0f;
        items.Add(new SequenceItem
        {
            startTime = lastStartTime,
            transition = transition
        });
        totalDuration = Mathf.Max(totalDuration, lastStartTime + transition.Duration);
        return this;
    }

    public USSSequence Insert(float atTime, USSTransition transition)
    {
        items.Add(new SequenceItem
        {
            startTime = atTime,
            transition = transition
        });
        totalDuration = Mathf.Max(totalDuration, atTime + transition.Duration);
        return this;
    }

    public USSSequence AppendInterval(float interval)
    {
        currentDuration += interval;
        totalDuration = Mathf.Max(totalDuration, currentDuration);
        return this;
    }

    public USSSequence PrependInterval(float interval)
    {
        for (int i = 0; i < items.Count; i++)
        {
            SequenceItem item = items[i];
            item.startTime += interval;
            items[i] = item;
        }
        for (int i = 0; i < callbacks.Count; i++)
        {
            SequenceCallback cb = callbacks[i];
            cb.time += interval;
            callbacks[i] = cb;
        }
        currentDuration += interval;
        totalDuration += interval;
        return this;
    }

    public USSSequence AppendCallback(Action callback)
    {
        callbacks.Add(new SequenceCallback
        {
            time = currentDuration,
            callback = callback
        });
        return this;
    }

    public USSSequence InsertCallback(float atTime, Action callback)
    {
        callbacks.Add(new SequenceCallback
        {
            time = atTime,
            callback = callback
        });
        return this;
    }

    public USSSequence SetLoops(int loopCount, LoopType type = LoopType.Restart)
    {
        loops = loopCount;
        loopType = type;
        return this;
    }

    public USSSequence OnComplete(Action callback)
    {
        onComplete = callback;
        return this;
    }

    public USSSequence OnStart(Action callback)
    {
        onStart = callback;
        return this;
    }

    public USSSequence OnUpdate(Action callback)
    {
        onUpdate = callback;
        return this;
    }

    public USSSequence Play()
    {
        if (isPlaying && !isPaused)
        {
            return this;
        }

        if (items.Count == 0)
        {
            return this;
        }

        if (isPaused)
        {
            isPaused = false;
            foreach (var transition in activeTransitions)
            {
                transition.Resume();
            }
            if (scheduledItem != null)
            {
                scheduledItem.Resume();
            }
            return this;
        }

        isPlaying = true;
        elapsedTime = 0f;
        currentLoop = 0;
        startedItems.Clear();
        executedCallbacks.Clear();
        activeTransitions.Clear();

        onStart?.Invoke();
        
        VisualElement schedulerHost = items[0].transition.Target;
        startTime = Time.time;
        scheduledItem = schedulerHost.schedule.Execute(UpdateSequence).Every(16);

        return this;
    }

    void UpdateSequence()
    {
        if (isPaused)
        {
            return;
        }

        elapsedTime = Time.time - startTime;

        for (int i = 0; i < items.Count; i++)
        {
            if (!startedItems.Contains(i) && elapsedTime >= items[i].startTime)
            {
                startedItems.Add(i);
                activeTransitions.Add(items[i].transition);
                items[i].transition.Play();
            }
        }

        for (int i = 0; i < callbacks.Count; i++)
        {
            if (!executedCallbacks.Contains(i) && elapsedTime >= callbacks[i].time)
            {
                executedCallbacks.Add(i);
                callbacks[i].callback?.Invoke();
            }
        }

        onUpdate?.Invoke();

        if (elapsedTime >= totalDuration)
        {
            currentLoop++;
            
            bool shouldContinue = (loops == -1) || (loops > 0 && currentLoop < loops);
            
            if (shouldContinue)
            {
                if (loopType == LoopType.Restart)
                {
                    startedItems.Clear();
                    executedCallbacks.Clear();
                    activeTransitions.Clear();
                    startTime = Time.time;
                    elapsedTime = 0f;
                }
                else
                {
                    CompleteSequence();
                }
            }
            else
            {
                CompleteSequence();
            }
        }
    }

    void CompleteSequence()
    {
        if (scheduledItem != null)
        {
            scheduledItem.Pause();
            scheduledItem = null;
        }
        activeTransitions.Clear();
        isPlaying = false;
        onComplete?.Invoke();
    }

    public void Pause()
    {
        isPaused = true;
        if (scheduledItem != null)
        {
            scheduledItem.Pause();
        }
        foreach (var transition in activeTransitions)
        {
            transition.Pause();
        }
    }

    public void Resume()
    {
        if (isPaused)
        {
            isPaused = false;
            startTime = Time.time - elapsedTime;
            if (scheduledItem != null)
            {
                scheduledItem.Resume();
            }
            foreach (var transition in activeTransitions)
            {
                transition.Resume();
            }
        }
    }

    public void Kill()
    {
        if (scheduledItem != null)
        {
            scheduledItem.Pause();
            scheduledItem = null;
        }
        foreach (var transition in activeTransitions)
        {
            transition.Kill();
        }
        activeTransitions.Clear();
        isPlaying = false;
        isPaused = false;
    }

    public void Complete()
    {
        Kill();
        foreach (var item in items)
        {
            item.transition.Complete();
        }
        onComplete?.Invoke();
    }
}