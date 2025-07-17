using UnityEngine;

[System.Serializable]
public struct Ingredient
{
    public Item item;
    public int amount;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Game/Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeName;
    public Ingredient[] ingredients;
    public Ingredient[] results;
}
