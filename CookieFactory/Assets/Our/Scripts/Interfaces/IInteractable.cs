using UnityEngine;

public interface IInteractable
{
    bool CanInteract();

    void Interact();

    void CloseMenu();

    // (Optional) What text to show on “E” (e.g. “Mix” vs “Too cold”)
    string GetInteractionText();
}
