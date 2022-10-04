using UnityEngine;


[CreateAssetMenu(fileName = "PrefabSettings", menuName = "ScriptableObjects/Prefabs")]
public class PrefabSettings : ScriptableObject {
    
    [SerializeField]
    private GameObject _fruitPrefab;
    [SerializeField]
    private GameObject _backgroundPrefab;

    public GameObject GetFruitPrefab() {
        return _fruitPrefab;
    }

    public GameObject GetBackgroundPrefab() {
        return _backgroundPrefab;
    }
    
}

