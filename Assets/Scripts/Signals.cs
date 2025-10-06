using System;

public static class Signals
{
    public static Action OnStartAnimation;
    public static Action OnSpinComplete;
    public static Action OnReelSpinStart;
    public static Action OnReset;
    public static Action OnParticlesComplete;
    public static Action OnPanelShowComplete;
    public static Action<string> OnSlotWheelTargetSelected;
}