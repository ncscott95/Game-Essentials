using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageHitbox : MonoBehaviour
{
    protected int _damage;
    protected LayerMask _targetLayers;
    private Collider _collider;

    public virtual void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
    }

    public void InitializeHitbox(int damage, LayerMask targetLayers)
    {
        _damage = damage;
        _targetLayers = targetLayers;
    }

    public void SetHitboxActive(bool active)
    {
        if (_collider == null) Debug.LogError("Hitbox Missing Collider");
        _collider.enabled = active;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other} hit, {other.gameObject.layer}, {_targetLayers}, {((1 << other.gameObject.layer) & _targetLayers) != 0}, {other.GetComponent<IDamageable>() != null}");
        if (((1 << other.gameObject.layer) & _targetLayers) != 0 && other.TryGetComponent<IDamageable>(out var hit))
        {
            hit.TakeDamage(_damage);
        }
    }
}
