using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ClassRandomizer : MonoBehaviour
{
    public Dictionary<string, Color> classColors = new() {
        { "Death Knight", new Color(0.77f, 0.12f, 0.23f) },
        { "Druid", new Color(1.0f, 0.49f, 0.04f) },
        { "Hunter", new Color(0.67f, 0.83f, 0.45f) },
        { "Mage", new Color(0.25f, 0.78f, 0.92f) },
        { "Paladin", new Color(0.96f, 0.55f, 0.73f) },
        { "Priest", new Color(1.0f, 1.0f, 1.0f) },
        { "Rogue", new Color(1.0f, 0.96f, 0.41f) },
        { "Shaman", new Color(0.0f, 0.44f, 0.87f) },
        { "Warlock", new Color(0.53f, 0.53f, 0.93f) },
        { "Warrior", new Color(0.78f, 0.61f, 0.43f) }
    };
    
    // Start is called before the first frame update
    void Start()
    {
        // Randomly assign a class color
        var spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer.color == new Color(0,0,0))
        {
            spriteRenderer.color = classColors.Values.ElementAt(Random.Range(0, classColors.Count));
        }
    }
}
