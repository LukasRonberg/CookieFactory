using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Text;

public class Mixer : MonoBehaviour, IInteractable, IItemReceiver
{
    [SerializeField] Animator animator;
    [SerializeField] Recipe[] recipes;
    [SerializeField] GameObject recipePanel;
    [SerializeField] Button[] recipeButton;

    //private List<Item> insertedItems = new List<Item>(); // items dropped into the mixer
    private List<Ingredient> insertedItems = new List<Ingredient>();

    private bool isMixing = false;
    private bool isSelectingRecipe = false;
    private Recipe currentRecipe;

    public Recipe CurrentRecipe => currentRecipe;


    public void Awake()
    {
        recipePanel.SetActive(false);
        // for each button, hook up its click to pick that index
        for (int i = 0; i < recipeButton.Length; i++)
        {
            int index = i; // capture for the closure
            recipeButton[i].onClick.AddListener(() => OnRecipeButtonClicked(index));
        }
    }
    private void OnRecipeButtonClicked(int index)
    {
        if (index < 0 || index >= recipes.Length) return;

        currentRecipe = recipes[index];
        isSelectingRecipe = false;          // done selecting
        recipePanel.SetActive(false);       // optionally hide the panel
        InputLock.SetLocked(true);
        Debug.Log($"Selected recipe: {currentRecipe.recipeName}");

        // now you can fire off whatever happens next, e.g. start mixing:
        //StartMixing();
    }

    /// Call this when the player drops or uses an item on the mixer.
    /// Returns true if the mixer accepted the item.
    public bool InsertItem(Item heldItem, int amount = 1)
    {
        Debug.Log("1st");
        if (isMixing || currentRecipe == null)
            return false; // can’t add mid-mix or with no recipe selected
        Debug.Log("2nd");
        // Find the requirement for this exact SO in the current recipe
        var req = currentRecipe.ingredients
                     .FirstOrDefault(i => i.item == heldItem);
        if (req.item == null)
            return false; // this recipe doesn’t use that item

        // How many of that SO have we already inserted?
        int alreadyInserted = insertedItems
            .Where(i => i.item == heldItem)
            .Sum(i => i.amount);
        Debug.Log("3rd");
        // Don’t allow over-inserting beyond what the recipe needs
        if (alreadyInserted + amount > req.amount)
            return false;
        Debug.Log("4th");
        // All good—wrap it as an Ingredient and add it
        insertedItems.Add(new Ingredient
        {
            item = heldItem,
            amount = amount
        });
        return true;
    }

    /// Returns the text to display to the player.
    /// 

    public bool HasRecipe()
    {
        if (currentRecipe != null)
        {
            return true;
        }
        else return false;
    }



    public string GetInteractionText()
    {
        // 1) Mixing done?
        if (isMixing)
            return "Collect (E)";

        // 2) In recipe‐selection panel?
        if (isSelectingRecipe)
            return string.Empty;

        // 3) No recipe chosen?
        if (currentRecipe == null)
            return "Select Recipes (E)";

        // 4) Build exactly the lines you want
        var sb = new StringBuilder();
        sb.AppendLine(currentRecipe.recipeName + ":");

        foreach (var req in currentRecipe.ingredients)
        {
            // Sum up how much of this Item SO you’ve inserted
            int have = insertedItems
                .Where(ins => ins.item == req.item)
                .Sum(ins => ins.amount);

            // Use the SO’s name (or id/displayName if you added one)
            string displayName = req.item.name;

            sb.AppendLine($"{displayName} {have}/{req.amount}");
        }
        // Trim the trailing newline
        return sb.ToString().TrimEnd();
    }



    /*public string GetInteractionText()
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
    }*/

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
        if (currentRecipe == null)
        {
            InputLock.SetLocked(false);
            recipePanel.gameObject.SetActive(true);
            isSelectingRecipe = true;
        }




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
