using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class Mixer : MonoBehaviour, IInteractable
{
    [SerializeField] Animator animator;
    [SerializeField] Recipe[] recipes;  // configure Flour+Water?Dough, Butter+Sugar+Egg?CookieDough, etc.
    [SerializeField] GameObject recipePanel;
    [SerializeField] Button recipeButton;

    private List<Item> insertedItems = new List<Item>(); // items dropped into the mixer
    private bool isMixing = false;
    private bool isSelectingRecipe = false;
    private Recipe currentRecipe;


    public void Awake()
    {
        recipePanel.SetActive(false);
        recipeButton.onClick.AddListener(OnRecipeButtonClicked);
    }
    private void OnRecipeButtonClicked()
    {
        Debug.Log("Test!");
    }

    /// Call this when the player drops or uses an item on the mixer.
    /// Returns true if the mixer accepted the item.
    public bool InsertItem(Item item)
    {
        if (isMixing)
            return false; // cannot insert while mixing is in progress

        bool isNeeded = recipes.Any(recipe =>
            !insertedItems.Any(existingItem => existingItem.name == item.name)
            && recipe.ingredients.Any(ingredient => ingredient.name == item.name && ingredient.amount == item.amount)
        );

        if (!isNeeded)
            return false;

        insertedItems.Add(item);
        return true;
    }

    /// Returns the text to display to the player.
    public string GetInteractionText()
    {
        if (!isMixing)
        {
            if (isSelectingRecipe)
                return "";
            if (insertedItems.Count == 0)
                return "Select Recipes (E)";

            // if they've added something but not a valid full set yet
            return FindMatchingRecipe() != null
                ? "Mix"
                : "Insert Ingredients";
        }
        else
        {
            return "Collect";
        }
    }

    /// Controls whether the player can interact with the mixer (Mix or Collect).
    public bool CanInteract()
    {
        if (isMixing)
            //return FindMatchingRecipe() != null;
            return false;
        else
            return true;
    }


    /// Handles the Mix action (start mixing) and the Collect action (spawn results).

    public void Interact()
    {
        InputLock.SetLocked(false);
        if (!InputLock.IsLocked)
        {
            recipePanel.gameObject.SetActive(true);
            isSelectingRecipe = true;
        }

        
        
        Debug.Log("Test to see if locked");
        /*
        if (!isMixing)
        {
            currentRecipe = FindMatchingRecipe();
            if (currentRecipe == null)
                return;

            animator.SetTrigger("StartMix");
            isMixing = true;
        }
        else
        {
            foreach (var resultItem in currentRecipe.results)
                SpawnResult(resultItem);

            insertedItems.Clear();
            isMixing = false;
            currentRecipe = null;
        }
        */
    }


    /// Checks if the set of inserted items exactly matches one recipe's ingredients.
    private Recipe FindMatchingRecipe()
    {
        foreach (var recipe in recipes)
        {
            if (recipe.ingredients.Length != insertedItems.Count)
                continue;

            bool isMatch = recipe.ingredients.All(ingredient =>
                insertedItems.Any(existingItem => existingItem.name == ingredient.name && existingItem.amount == ingredient.amount)
            );

            if (isMatch)
                return recipe;
        }
        return null;
    }

    /// Spawns or gives the result item(s) when mixing is complete.
    private void SpawnResult(Item resultItem)
    {
        // TODO: implement your game’s logic to spawn or grant the result item
        // e.g. Instantiate(pickupPrefab, outputSlot.position, Quaternion.identity)
    }

    public void CloseMenu()
    {
        isSelectingRecipe = false;
        recipePanel.gameObject.SetActive(false);
    }
}
