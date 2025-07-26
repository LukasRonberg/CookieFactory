using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class Shaper : MonoBehaviour, IInteractable, IItemReceiver
{
    [SerializeField] Animator animator;
    [SerializeField] Recipe[] recipes;
    [SerializeField] GameObject recipePanel;
    [SerializeField] Button[] recipeButton;
    [SerializeField] private float shapeDuration = 5f;

    //private List<Item> insertedItems = new List<Item>(); // items dropped into the mixer
    private List<Ingredient> insertedItems = new List<Ingredient>();

    private bool shapingInProgress = false;
    private bool canCollect = false;
    private bool isSelectingRecipe = false;
    private Recipe currentRecipe;
    private bool readyToMix = false;

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
        if (shapingInProgress || currentRecipe == null || canCollect)
            return false; // can’t add mid-mix or with no recipe selected
        // Find the requirement for this exact SO in the current recipe
        var req = currentRecipe.ingredients
                     .FirstOrDefault(i => i.item == heldItem);
        if (req.item == null)
            return false; // this recipe doesn’t use that item

        // How many of that SO have we already inserted?
        int alreadyInserted = insertedItems
            .Where(i => i.item == heldItem)
            .Sum(i => i.amount);
        // Don’t allow over-inserting beyond what the recipe needs
        if (alreadyInserted + amount > req.amount)
            return false;
        // All good—wrap it as an Ingredient and add it
        insertedItems.Add(new Ingredient
        {
            item = heldItem,
            amount = amount
        });
        RecalculateReady();
        return true;
    }

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
        if (shapingInProgress) return "Mixing…";      // timer running
        if (canCollect) return "Collect (E)";  // finished
        if (readyToMix) return "Mix (E)";      // all ingredients present
        if (currentRecipe == null) return "Select Recipes (E)";

        // 5) Otherwise build progress lines
        var sb = new StringBuilder();
        sb.AppendLine(currentRecipe.recipeName + ":");

        foreach (var req in currentRecipe.ingredients)
        {
            int have = insertedItems
                .Where(ins => ins.item == req.item)
                .Sum(ins => ins.amount);

            sb.AppendLine($"{req.item.name} {have}/{req.amount}");
        }

        return sb.ToString().TrimEnd();
    }


    /// Controls whether the player can interact with the mixer (Mix or Collect).
    public bool CanInteract()
    {
        if (shapingInProgress)
            //return FindMatchingRecipe() != null;
            return false;
        else
            return true;
    }


    /// Handles the Mix action (start mixing) and the Collect action (spawn results).
    public void Interact()
    {
        // 1) Collect phase  ─────────────────────────────
        if (canCollect)
        {
            foreach (var result in currentRecipe.results)
                SpawnResult(result);

            insertedItems.Clear();
            currentRecipe = null;
            canCollect = false;
            RecalculateReady();
            return;
        }

        // 2) Ignore clicks while the timer is running ───
        if (shapingInProgress)
            return;

        // 3) Start-mix phase (all ingredients present) ──
        if (readyToMix)                          // see earlier answer for readyToMix
        {
            StartCoroutine(ShapeRoutine());        // kicks off 5-second wait
            return;
        }

        // 4) Not mixing yet
        if (currentRecipe == null)          // open the recipe chooser again
        {
            InputLock.SetLocked(false);
            recipePanel.gameObject.SetActive(true);
            isSelectingRecipe = true;
            return;
        }

        if (currentRecipe != null)          // open the recipe chooser again
        {
            InputLock.SetLocked(false);
            recipePanel.gameObject.SetActive(true);
            isSelectingRecipe = true;
            return;
        }

    }

    /// Spawns or gives the result item(s) when mixing is complete.
    private ItemInteractable SpawnResult(Ingredient ingredient)
    {
        if (ingredient.item.prefab == null)
        {
            Debug.LogWarning($"{ingredient.item.name} has no prefab assigned.");
            return null;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 0.4f;   // tweak as you like
        GameObject go = Instantiate(ingredient.item.prefab, spawnPos, Quaternion.identity);

        // (Optional) override the quantity the prefab was authored with
        var item = go.GetComponent<ItemInteractable>();
        if (item != null)
        {
            item.Quantity = ingredient.amount;
            var player = FindFirstObjectByType<InteractWithMachines>();   // or cache / inject this reference
            player?.ForcePickup(item);
        }
        return item;
    }


    public void CloseMenu()
    {
        isSelectingRecipe = false;
        recipePanel.gameObject.SetActive(false);
    }
    private IEnumerator ShapeRoutine()
    {
        shapingInProgress = true;           // block other interactions
        animator.SetTrigger("Shape");

        yield return new WaitForSeconds(shapeDuration);

        animator.SetTrigger("Shape");        // back to “Idle/Done” state
        shapingInProgress = false;
        canCollect = true;                   // NOW the player may press E again
    }


    private void RecalculateReady()
    {
        // ready only when a recipe is chosen *and* every ingredient quota is met
        readyToMix = currentRecipe != null &&
                     currentRecipe.ingredients.All(req =>
                         insertedItems.Where(i => i.item == req.item)
                                      .Sum(i => i.amount) >= req.amount);
    }
}
