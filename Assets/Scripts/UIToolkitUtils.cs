using UnityEngine.UIElements;

public class UIToolkitUtils
{
    
    public static VisualElement Create(params string[] classNames)
    {
        return Create<VisualElement>(classNames);
    }

    public static T Create<T>(params string[] classNames) where T : VisualElement, new()
    {
        T element = new T();
        foreach (var className in classNames)
        {
            element.AddToClassList(className);
        }

        return element;
    }
}