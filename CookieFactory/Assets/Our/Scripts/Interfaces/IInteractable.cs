using UnityEngine;

public interface IInteractable
{
    // Should we allow the interaction right now?
    bool CanInteract();

    // Perform the interaction.
    void Interact();

    // (Optional) What text to show on “E” (e.g. “Mix” vs “Too cold”)
    string GetInteractionText();
}
