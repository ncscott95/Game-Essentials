namespace InventorySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewEquippable", menuName = "Inventory/Equippables/Generic Equippable", order = 0)]
    public abstract class InventoryObject : ScriptableObject
    {
        public string Name;
        public string Description;
        public Sprite Icon;

        public static InventoryObject NewObject(string name, string description, Sprite icon)
        {
            InventoryObject newEquippable = CreateInstance<InventoryObject>();

            newEquippable.Name = name;
            newEquippable.Description = description;
            newEquippable.Icon = icon;

            return newEquippable;
        }

        public virtual void OnEquip()
        {
            Debug.Log("Equipped item: " + Name);
        }

        public virtual void OnUnequip()
        {
            Debug.Log("Unequipped item: " + Name);
        }
    }
}
