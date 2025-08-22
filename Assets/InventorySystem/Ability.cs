namespace InventorySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewAbility", menuName = "Inventory/Equippables/Ability", order = 3)]
    public class Ability : InventoryObject
    {
        public AbilitySystem.Ability AbilityScript;

        public static Ability NewAbility(string name, string description, Sprite icon)
        {
            Ability newAbility = CreateInstance<Ability>();
            newAbility.Name = name;
            newAbility.Description = description;
            newAbility.Icon = icon;
            return newAbility;
        }
    }
}
