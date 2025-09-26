using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class CurvedText : VisualElement
{
    List<Label> characterLabels;
    string text = "";
    float radius = 200f;
    float arcAngle = 15f; // Much smaller for tight arch
    float characterSpacing = 1f;
    bool curveUp = true;
    
    public string Text
    {
        get => text;
        set { text = value; UpdateCurve(); }
    }
    
    public float Radius
    {
        get => radius;
        set { radius = value; UpdateCurve(); }
    }
    
    public float ArcAngle
    {
        get => arcAngle;
        set { arcAngle = value; UpdateCurve(); }
    }
    
    public float CharacterSpacing
    {
        get => characterSpacing;
        set { characterSpacing = value; UpdateCurve(); }
    }
    
    public bool CurveUp
    {
        get => curveUp;
        set { curveUp = value; UpdateCurve(); }
    }

    public CurvedText()
    {
        characterLabels = new List<Label>();
        style.position = Position.Relative;
        style.width = Length.Percent(100);
        style.height = Length.Percent(100);
    }

    public CurvedText(string text, float radius = 400f, float arcAngle = 15f) : this()
    {
        this.text = text;
        this.radius = radius;
        this.arcAngle = arcAngle;
        UpdateCurve();
    }
    
    public List<Label> GetLabels()
    {
        return characterLabels;
    }

    void UpdateCurve()
    {
        ClearCharacters();
        CreateCharacterLabels();
        
        // Wait for geometry update
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    void OnGeometryChanged(GeometryChangedEvent evt)
    {
        this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        PositionCharacters();
    }

    void ClearCharacters()
    {
        foreach (var label in characterLabels)
        {
            Remove(label);
        }
        characterLabels.Clear();
    }

    void CreateCharacterLabels()
    {
        foreach (char character in text)
        {
            var label = new Label("<b>" + character + "</b>")
            {
                style =
                {
                    position = Position.Absolute,
                    transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50)),
                    fontSize = 24
                }
            };
            
            label.AddToClassList("curved-text");
            
            characterLabels.Add(label);
            Add(label);
        }
    }

    void PositionCharacters()
    {
        if (characterLabels.Count == 0) return;

        float containerWidth = resolvedStyle.width;
        float containerHeight = resolvedStyle.height;
        
        if (containerWidth == 0 || containerHeight == 0) return;
        
        float centerX = containerWidth * 0.5f;
        float centerY = containerHeight * 0.5f;
        
        // Calculate total angle in radians
        float totalAngleRadians = arcAngle * Mathf.Deg2Rad;
        
        // Calculate spacing between characters
        float spacing = totalAngleRadians / Mathf.Max(1, characterLabels.Count - 1);
        spacing *= characterSpacing;
        
        // Start from the leftmost position
        float startAngle = -totalAngleRadians * 0.5f;
        
        for (int i = 0; i < characterLabels.Count; i++)
        {
            var label = characterLabels[i];
            float angle = startAngle + (spacing * i);
            
            // Calculate position on the arc
            float x = centerX + Mathf.Sin(angle) * radius;
            float y = centerY + Mathf.Cos(angle) * radius * (curveUp ? -1 : 1);
            
            // Center the character
            label.style.left = x - (label.resolvedStyle.width * 0.5f);
            label.style.top = y - (label.resolvedStyle.height * 0.5f);
            
            // Rotate character to follow the curve
            float rotationDegrees = angle * Mathf.Rad2Deg * (curveUp ? 1 : -1);
            label.style.rotate = new Rotate(rotationDegrees);
        }
    }

    public void SetFontSize(float fontSize)
    {
        foreach (var label in characterLabels)
        {
            label.style.fontSize = fontSize;
        }
    }
    
    public void SetTextColor(Color color)
    {
        foreach (var label in characterLabels)
        {
            label.style.color = color;
        }
    }
}