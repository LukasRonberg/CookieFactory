using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
public class Item : ScriptableObject
{
    [Tooltip("Internal ID; example: 'flour'")]
    public string id;
    [Tooltip("Prefab to spawn when you create this item")]
    public GameObject prefab;
    [Tooltip("Icon for UI, etc.")]
    public Sprite icon;
}
