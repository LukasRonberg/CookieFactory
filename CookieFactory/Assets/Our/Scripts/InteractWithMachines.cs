using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InteractWithMachines : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 5f; // How far to check if object is interactable
    public TMP_Text interactionText;
    public string interactableTag = "Machine";
    [Tooltip("Optional layer mask for performance")]
    public LayerMask layerMask = ~0;  // default = everything

    private IInteractable currentInteractable;
    void Start()
    {
        interactionText.gameObject.SetActive(false);

        // Only include the "Interactable" layer in our mask
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        layerMask = 1 << interactableLayer;

    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask) && IsTaggedInteractable(hit.transform))
        {      
            currentInteractable = hit.transform.GetComponentInParent<IInteractable>();
            if (currentInteractable != null && currentInteractable.CanInteract())
            {
                Debug.Log("Current Interactable: " + currentInteractable);
                interactionText.text = $"{currentInteractable.GetInteractionText()}";
                interactionText.gameObject.SetActive(true);

                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    currentInteractable.Interact();
                } else if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    currentInteractable.CloseMenu();
                }
            }
        }
        else
        {
            interactionText.gameObject.SetActive(false);
            currentInteractable = null;
        }
    }

    // Tjekker efter Top parent i et objekt for at se om det har det korrekte Tag
    bool IsTaggedInteractable(Transform t)
    {
        while (t != null)
        {
            if (t.CompareTag(interactableTag))
                return true;
            t = t.parent;
        }
        return false;
    }
}
