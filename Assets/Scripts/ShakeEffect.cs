using UnityEngine;
using UnityEngine.UIElements;

public class ShakeEffect : VisualElement
{
    float shakeDuration = 3f;
    float shakeIntensity = 20f;
    float shakeStepDuration = 0.05f; // Fixed step duration for intense shaking

    VisualElement targetElement;
    bool isShaking;
    int currentShakeStep;
    int totalShakeSteps;

    public ShakeEffect(VisualElement targetElement)
    {
        this.targetElement = targetElement;
        targetElement.AddToClassList("shake-element");
    }

    public void TriggerShake()
    {
        if (targetElement == null) return;

        StopCurrentShake();
        StartShake();
    }

    public void TriggerShake(float duration, float intensity)
    {
        shakeDuration = duration;
        shakeIntensity = intensity;
        TriggerShake();
    }
    
    public void TriggerShake(float duration, float intensity, float stepDuration)
    {
        shakeDuration = duration;
        shakeIntensity = intensity;
        shakeStepDuration = stepDuration;
        TriggerShake();
    }

    void StartShake()
    {
        targetElement.AddToClassList("shake-element--shaking");

        isShaking = true;
        currentShakeStep = 0;
        totalShakeSteps = Mathf.RoundToInt(shakeDuration / shakeStepDuration);

        // Start the shake sequence
        ExecuteShakeStep();
    }

    void ExecuteShakeStep()
    {
        if (!isShaking) return; // Safety check in case shake was stopped

        if (currentShakeStep < totalShakeSteps)
        {
            float progress = (float)currentShakeStep / totalShakeSteps;
            float dampening = 1f - progress;

            float offsetX = Random.Range(-shakeIntensity, shakeIntensity) * dampening;
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity) * dampening;

            targetElement.style.translate = new Translate(offsetX, offsetY);

            currentShakeStep++;

            // Schedule next shake step
            targetElement.schedule.Execute(ExecuteShakeStep)
                .StartingIn((long)(shakeStepDuration * 1000)); // Convert to milliseconds
        }
        else
        {
            // Shake complete, return to original position
            CompleteShake();
        }
    }

    void CompleteShake()
    {
        if (!isShaking) return;

        targetElement.style.translate = new Translate(0, 0);

        // Schedule removal of shaking class after a brief delay
        targetElement.schedule.Execute(() =>
        {
            targetElement.RemoveFromClassList("shake-element--shaking");
            isShaking = false;
        }).StartingIn(100); // 100ms delay
    }

    void StopCurrentShake()
    {
        isShaking = false;

        // Reset position and classes immediately
        if (targetElement != null)
        {
            targetElement.style.translate = new Translate(0, 0);
            targetElement.RemoveFromClassList("shake-element--shaking");
        }
    }

    public void SetTarget(VisualElement element)
    {
        StopCurrentShake();

        targetElement = element;
        if (targetElement != null)
        {
            targetElement.AddToClassList("shake-element");
        }
    }
}