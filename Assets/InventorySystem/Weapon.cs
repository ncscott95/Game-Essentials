namespace InventorySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Equippables/Weapon", order = 2)]
    public class Weapon : InventoryObject
    {
        public int Damage;

        public static Weapon NewWeapon(string name, string description, Sprite icon, int damage)
        {
            Weapon newWeapon = CreateInstance<Weapon>();
            newWeapon.Name = name;
            newWeapon.Description = description;
            newWeapon.Icon = icon;
            newWeapon.Damage = damage;
            return newWeapon;
        }
    }
}
