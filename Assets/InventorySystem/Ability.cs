namespace InventorySystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewAbility", menuName = "Inventory/Equippables/Ability", order = 3)]
    public class Ability : Equippable
    {
        public int ManaCost;

        public static Ability NewAbility(string name, string description, Sprite icon, int manaCost)
        {
            Ability newAbility = CreateInstance<Ability>();
            newAbility.Name = name;
            newAbility.Description = description;
            newAbility.Icon = icon;
            newAbility.ManaCost = manaCost;
            return newAbility;
        }
    }
}
