using System.Collections.Generic;
using UnityEngine;
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
    
    public static void WrapListEdges<T>(List<T> list)
    {
        if (list.Count < 2) return;
    
        T first = list[0];
        T last = list[list.Count - 1];
    
        list.Add(first);
        list.Insert(0, last);
    }
    
    public static void DuplicateListItems<T>(List<T> list)
    {
        int originalCount = list.Count;
        for (int i = 0; i < originalCount; i++)
        {
            list.Add(list[i]);
        }
    }
}