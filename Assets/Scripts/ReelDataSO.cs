using UnityEngine;

[CreateAssetMenu(fileName = "ReelDataSO", menuName = "Data/ReelData", order = 0)]
public class ReelDataSO : ScriptableObject
{
    [Header("Spin Settings")]
    public float initialSpeedMultiplier = 1f;
    public float speedIncrement = 0.03f;
    public float baseSpeed = 2f;
    public int totalSpinLoops = 6;
    
    [Header("Timing")]
    public int spinScheduleIntervalMs = 10;
    public int effectStartDelayMs = 1000;
    public float stopSpinDuration = 1f;
    public float soundFadeDuration = 0.5f;
    
    [Header("Effect Settings")]
    public float sparkleEffectDuration = 0.07f;
    public int sparkleLoopCount = 2;
    public float flashEffectDuration = 0.07f;
    public int flashLoopCount = 20;
}