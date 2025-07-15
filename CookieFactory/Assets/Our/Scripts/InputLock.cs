using UnityEngine;

public static class InputLock
{

    public static bool IsLocked { get; private set; }

    public static void SetLocked(bool locked)
    {
        IsLocked = locked;

        // lock / hide when locked, unlock / show when unlocked
        Cursor.lockState = locked
            ? CursorLockMode.Locked
            : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
