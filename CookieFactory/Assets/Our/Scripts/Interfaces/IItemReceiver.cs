using UnityEngine;

public interface IItemReceiver
{
    int InsertItem(Item item, int amount);

    bool HasRecipe();

}
