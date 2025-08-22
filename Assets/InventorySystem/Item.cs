namespace InventorySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Equippables/Item", order = 1)]
    public class Item : Equippable, IStackable
    {
        public int StackCount { get; set; }
        public int MaxStackSize { get; set; }

        public int Value;

        public static Item NewItem(string name, string description, Sprite icon, int value)
        {
            Item newItem = CreateInstance<Item>();
            newItem.Name = name;
            newItem.Description = description;
            newItem.Icon = icon;
            newItem.Value = value;
            return newItem;
        }
    }
}
