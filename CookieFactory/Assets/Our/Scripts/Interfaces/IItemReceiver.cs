using UnityEngine;

public interface IItemReceiver
{
    bool InsertItem(Item item, int amount);

    bool HasRecipe();

}
