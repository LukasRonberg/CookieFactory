using UnityEngine;

public class ConveyorBeltScroll : MonoBehaviour
{
    [SerializeField] private Renderer beltRenderer;
    [SerializeField] private float scrollSpeed = 1.0f;

    private MaterialPropertyBlock _mpb;
    private float _offset;

    void Awake()
    {
        if (beltRenderer == null)
            beltRenderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        _offset += Time.deltaTime * scrollSpeed;
        beltRenderer.GetPropertyBlock(_mpb);
        _mpb.SetVector("_BaseMap_ST", new Vector4(1, 1, _offset, 0));
        beltRenderer.SetPropertyBlock(_mpb);
    }
}
