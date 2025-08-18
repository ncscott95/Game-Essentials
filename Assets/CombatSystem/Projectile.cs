using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    protected Attack _attack;
    private Rigidbody _rb;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _layerMask) != 0)
        {
            Debug.Log($"Collided with {other.gameObject.name}");
            OnCollide(other);
            Destroy(gameObject);
        }
    }

    public void InitializeProjectile(Attack attack, float initialForce)
    {
        _rb = GetComponent<Rigidbody>();
        _attack = attack;
        _rb.AddForce(transform.forward * initialForce, ForceMode.Impulse);
    }

    protected virtual void Update()
    {
        // Orient the projectile to face the direction of movement
        if (_rb.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);
        }
    }

    public abstract void OnCollide(Collider other);
}
