namespace InventorySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Equippables/Weapon", order = 2)]
    public class Weapon : Equippable
    {
        public int Damage;
    }
}
