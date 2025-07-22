using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("Which Item SO this prefab represents")]
    [SerializeField] private Item itemSO;
    [Tooltip("How many units the player picks up")]
    [SerializeField] private int quantity = 1;

    [Tooltip("If left empty the script will look for:  Player → Camera → child named HoldPoint")]
    [SerializeField] private Transform holdPoint;
    [Tooltip("Local position relative to holdPoint")]
    [SerializeField] private Vector3 holdLocalPos = Vector3.zero;
    [Tooltip("Local rotation relative to holdPoint")]
    [SerializeField] private Vector3 holdLocalEuler = Vector3.zero;

    private bool isHeld = false;

    public Item ItemSO => itemSO;
    public int Quantity
    {
        get => quantity; 
        set => quantity = value;
    } 
    public bool IsHeld
    {
        get => isHeld;
        set => isHeld = value;
    }
    private Rigidbody _rb;
    private Collider _col;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        if (holdPoint == null)
            holdPoint = FindHoldPoint();
    }

    public bool CanInteract() => true;

    public string GetInteractionText()
    {
        if (!isHeld)
            return $"Pick up {itemSO.name} x{quantity}";
        else
            return "Drop";
    }

    public void Interact()
    {
        if (!isHeld)
        {
            Pickup();
        } 
        else
        {
            Drop();
        }
    }

    private void Pickup()
    {
        Debug.Log("Picked up item");
        // disable physics so it won't fall:
        if (_rb) _rb.isKinematic = true;
        // leave collider on so we can still raycast it for dropping

        // parent & position under camera
        if (holdPoint != null)
        {
            transform.SetParent(holdPoint, worldPositionStays: false);
            transform.localPosition = holdLocalPos;
            transform.localRotation = Quaternion.Euler(holdLocalEuler);
        }
        else
        {
            // fallback just in front of camera
            var cam = Camera.main;
            transform.SetParent(cam.transform, false);
            var vp = new Vector3(0.9f, 0.1f, 1f);
            transform.position = cam.ViewportToWorldPoint(vp);
            transform.rotation = cam.transform.rotation;
        }

        isHeld = true;
    }

    private void Drop()
    {
        Debug.Log("Dropped item");
        // unparent, keeping world position
        transform.SetParent(null, worldPositionStays: true);

        // re-enable physics so it falls
        if (_rb)
        {
            _rb.isKinematic = false;
            _rb.linearVelocity = Vector3.zero;  // clear any residual velocity
        }

        // collider was never disabled, so no need to re-enable
        isHeld = false;
    }

    /// Looks for GameObject tagged "Player" → first Camera in its hierarchy
    /// → child named "HoldPoint". Returns null if any step is missing.
    private Transform FindHoldPoint()
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning($"{name}: couldn’t find an object tagged <Player>.");
            return null;
        }

        var cam = player.GetComponentInChildren<Camera>();
        if (cam == null)
        {
            Debug.LogWarning($"{name}: player has no child Camera.");
            return null;
        }

        var hp = cam.transform.Find("HoldPoint");
        if (hp == null)
        {
            Debug.LogWarning($"{name}: camera has no child named <HoldPoint>.");
            return null;
        }

        return hp;
    }

    public void CloseMenu()
    {
        // Empty for items
    }
}
