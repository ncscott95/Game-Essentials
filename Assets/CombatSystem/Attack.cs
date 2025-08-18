using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Attack")]
public class Attack : ScriptableObject
{
    public int Damage;
    public float ChargeTime;
    public float HitActiveTime;
    public float CooldownTime;
    public LayerMask TargetLayers;
    [HideInInspector] public DamageHitbox Hitbox;
    [HideInInspector] public Action UseAttack;

    public void InitializeAttack(Action action, DamageHitbox hitbox = null)
    {
        UseAttack = action;
        if (hitbox != null)
        {
            Hitbox = hitbox;
            Hitbox.InitializeHitbox(Damage, TargetLayers);
        }
        else { Debug.Log("Missing Attack Hitbox"); }
    }

    public void SetHitboxActive(bool active)
    {
        if (Hitbox == null)
        {
            Debug.LogWarning($"That attack doesn't have a hitbox assigned!");
            return;
        }

        Hitbox.SetHitboxActive(active);
    }
}
