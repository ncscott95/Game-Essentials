namespace InventorySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewAbility", menuName = "Inventory/Equippables/Ability", order = 3)]
    public class Ability : Equippable
    {
        public int ManaCost;
    }
}
