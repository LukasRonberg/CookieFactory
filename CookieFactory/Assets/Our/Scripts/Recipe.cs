using UnityEngine;

[System.Serializable]
public class Recipe
{
    // inputs: array of Item (name + amount)
    public Item[] ingredients;

    // outputs: array of Item (name + amount)
    public Item[] results;
}
