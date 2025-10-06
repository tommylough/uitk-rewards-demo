using UnityEngine;

[CreateAssetMenu(fileName = "JuicyRewardsDataSO", menuName = "Data/JuicyRewardsData", order = 0)]
public class JuicyRewardsDataSO : ScriptableObject
{
    public ReelDataSO reelData;
    
    public float showRibbonDuration = 0.5f;
    public float showPanelTopTargetPercent = 40;
    public float showPanelTopDuration = 0.5f;

    public float ribbonTextDuration = 0.5f;
    public float ribbonTextScale = 1f;
    public float ribbonTextStaggerIncrement = 0.05f;
}