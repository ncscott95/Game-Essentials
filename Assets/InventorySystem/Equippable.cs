namespace InventorySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewEquippable", menuName = "Inventory/Equippables/Generic Equippable", order = 0)]
    public abstract class Equippable : ScriptableObject
    {
        public string Name;
        public string Description;
        public Sprite Icon;
    }
}
