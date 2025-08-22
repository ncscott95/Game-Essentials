namespace InventorySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewEquippable", menuName = "Inventory/Equippables/Generic Equippable", order = 0)]
    public abstract class Equippable : ScriptableObject
    {
        public string Name;
        public string Description;
        public Sprite Icon;

        public static Equippable NewEquippable(string name, string description, Sprite icon)
        {
            Equippable newEquippable = CreateInstance<Equippable>();

            newEquippable.Name = name;
            newEquippable.Description = description;
            newEquippable.Icon = icon;

            return newEquippable;
        }
    }
}
