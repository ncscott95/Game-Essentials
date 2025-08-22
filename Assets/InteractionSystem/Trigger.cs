using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    public bool DestroyOnTrigger = true;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerMask) != 0)
        {
            OnEnter(other);
            if (DestroyOnTrigger) Destroy(transform.parent.gameObject);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerMask) != 0)
        {
            OnExit(other);
            if (DestroyOnTrigger) Destroy(transform.parent.gameObject);
        }
    }

    public virtual void OnEnter(Collider other) { }
    public virtual void OnExit(Collider other) { }
}
