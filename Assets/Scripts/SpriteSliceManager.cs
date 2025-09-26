using System.Collections.Generic;
using UnityEngine;

public static class SpriteSliceManager
{
    static List<Sprite> sprites;
    
    public static void Initialize(List<Sprite> spriteArray)
    {
        sprites = new List<Sprite>(spriteArray);
    }
    
    public static Sprite GetSpriteSliceByName(string name)
    {
        if (sprites == null) return null;
        
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == name)
            {
                return sprite;
            }
        }
        
        return null;
    }
}