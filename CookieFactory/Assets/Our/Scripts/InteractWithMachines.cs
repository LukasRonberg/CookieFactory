using UnityEngine;

public class InteractWithMachines : MonoBehaviour
{
    public Camera playerCamera;
    public float maxDistance = 5f; // How far to check if object is interactable
    public GameObject eButtonUI;
    public string interactableTag = "Machine";
    [Tooltip("Optional layer mask for performance")]
    public LayerMask layerMask = ~0;  // default = everything


    void Start()
    {
        eButtonUI.SetActive(false);

        // Only include the "Interactable" layer in our mask
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        layerMask = 1 << interactableLayer;

    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            if (IsTaggedInteractable(hit.transform))
                eButtonUI.SetActive(true);
            else
                eButtonUI.SetActive(false);
        }
        else
        {
            eButtonUI.SetActive(false);
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
