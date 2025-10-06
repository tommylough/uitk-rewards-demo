using UnityEngine;
using UnityEngine.UIElements;

public static class USSTransitionExtensions
{
    public static USSTransition DOFade(this VisualElement element, float target, float duration)
    {
        return USSTransition.Opacity(element, target, duration);
    }

    public static USSTransition DOScale(this VisualElement element, Vector2 target, float duration)
    {
        return USSTransition.Scale(element, target, duration);
    }

    public static USSTransition DOScale(this VisualElement element, float target, float duration)
    {
        return USSTransition.Scale(element, new Vector2(target, target), duration);
    }

    public static USSTransition DORotate(this VisualElement element, float degrees, float duration)
    {
        return USSTransition.Rotate(element, degrees, duration);
    }

    public static USSTransition DOMove(this VisualElement element, Vector2 target, float duration)
    {
        return USSTransition.Translate(element, target, duration);
    }

    public static USSTransition DOMoveX(this VisualElement element, float target, float duration)
    {
        return USSTransition.TranslateX(element, target, duration);
    }

    public static USSTransition DOMoveY(this VisualElement element, float target, float duration)
    {
        return USSTransition.TranslateY(element, target, duration);
    }

    public static USSTransition DOColor(this VisualElement element, Color target, float duration)
    {
        return USSTransition.Color(element, target, duration);
    }

    public static USSTransition DOBackgroundColor(this VisualElement element, Color target, float duration)
    {
        return USSTransition.BackgroundColor(element, target, duration);
    }

    public static USSTransition DOWidth(this VisualElement element, float target, float duration)
    {
        return USSTransition.Width(element, target, duration);
    }

    public static USSTransition DOHeight(this VisualElement element, float target, float duration)
    {
        return USSTransition.Height(element, target, duration);
    }

    public static USSTransition DOTop(this VisualElement element, float target, float duration)
    {
        return USSTransition.Top(element, target, duration);
    }

    public static USSTransition DOBottom(this VisualElement element, float target, float duration)
    {
        return USSTransition.Bottom(element, target, duration);
    }

    public static USSTransition DOLeft(this VisualElement element, float target, float duration)
    {
        return USSTransition.Left(element, target, duration);
    }

    public static USSTransition DORight(this VisualElement element, float target, float duration)
    {
        return USSTransition.Right(element, target, duration);
    }
}