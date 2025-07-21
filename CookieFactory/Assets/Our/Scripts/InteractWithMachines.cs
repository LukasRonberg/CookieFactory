using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractWithMachines : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 5f; // How far to check if object is interactable
    public TMP_Text interactionText;
    public string[] interactableTags = { "Machine", "Item" };
    [Tooltip("Optional layer mask for performance")]
    public LayerMask layerMask = ~0;  // default = everything

    private IInteractable currentInteractable;
    private ItemInteractable heldItem;
    void Start()
    {
        interactionText.gameObject.SetActive(false);

        // Only include the "Interactable" layer in our mask
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        layerMask = 1 << interactableLayer;

    }

    void Update()
    {
        // ──────────────────────────────────────
        // 1) Ray-cast first (so Debug.Log always fires)
        // ──────────────────────────────────────
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask) &&
            IsTaggedInteractable(hit.transform))
        {
            currentInteractable = hit.transform.GetComponentInParent<IInteractable>();

            if (currentInteractable != null)
            {
                Debug.Log("Current Interactable: " + currentInteractable);
                interactionText.text = currentInteractable.GetInteractionText();
                interactionText.gameObject.SetActive(true);
            }
        }
        else
        {
            currentInteractable = null;
            interactionText.gameObject.SetActive(false);
        }

        // ──────────────────────────────────────
        // 2) If we’re holding something, decide insert vs drop
        // ──────────────────────────────────────
        if (heldItem != null)
        {
            // Try to see whether the object we’re looking at can receive items
            IItemReceiver receiver = currentInteractable as IItemReceiver;

            // Can we insert right now?
            bool canInsert = receiver != null && receiver.HasRecipe();

            // UI prompt
            interactionText.text = canInsert ? "Insert" : "Drop";
            interactionText.gameObject.SetActive(true);

            // Handle the E-key
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (canInsert &&
                    receiver.InsertItem(heldItem.ItemSO, heldItem.Quantity))
                {
                    // Insert succeeded → remove the world object
                    Destroy(heldItem.gameObject);
                }
                else
                {
                    // Either no receiver or receiver rejected → just drop it
                    heldItem.Interact(); // this calls Drop()
                }

                heldItem = null;
            }

            return; // skip rest of Update while holding
        }

        // ──────────────────────────────────────
        // 3) Normal pickup / machine interaction
        // ──────────────────────────────────────
        if (Keyboard.current == null) return;

        if (currentInteractable != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentInteractable.Interact();

            if (currentInteractable is ItemInteractable item && item.IsHeld)
                heldItem = item;
        }
        else if (currentInteractable != null &&
                 Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            currentInteractable.CloseMenu();
        }
    }

    // Tjekker efter Top parent i et objekt for at se om det har det korrekte Tag
    bool IsTaggedInteractable(Transform t)
    {
        while (t != null)
        {
            foreach (var tag in interactableTags)
                if (t.CompareTag(tag))
                    return true;

            t = t.parent;
        }
        return false;
    }
}
