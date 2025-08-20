namespace InventorySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Equippables/Item", order = 1)]
    public class Item : Equippable
    {
        public int Value;
    }
}
