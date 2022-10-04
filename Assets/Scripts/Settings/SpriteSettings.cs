using System.Collections.Generic;
using UnityEngine;
using System;



[CreateAssetMenu(fileName = "SpriteSettings", menuName = "ScriptableObjects/Sprites")]
public class SpriteSettings : ScriptableObject {
    
    [SerializeField]
    private List <FruitSprite> _fruitSprites;
    [SerializeField]
    private List <UIElementsSprite> _uiElements;
    [SerializeField]
    private List<FruitDestroySprites> _fruitDestroySprites;

    public Sprite GetFruitSprite(FRUIT type) {

        return _fruitSprites.Find(fruit => fruit.type == type).sprite;
    }

    public Sprite GetUIElementSprite(UIElement type) {
        return _uiElements.Find(element => element.type == type).sprite;
    }

    public List<Sprite> GetDestroySprites(FRUIT type) {
        return _fruitDestroySprites.Find(fruit => fruit.type == type).destroySprites;
    }

    [Serializable]
    public class FruitSprite {
        public FRUIT type;
        public Sprite sprite;
    }

    [Serializable]
    public class UIElementsSprite {
        public UIElement type;
        public Sprite sprite;
    }

    [Serializable]
    public class FruitDestroySprites {
        public FRUIT type;
        public List<Sprite> destroySprites;
    }

}
