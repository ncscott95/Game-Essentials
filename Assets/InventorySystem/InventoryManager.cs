using InventorySystem;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public Container<Weapon> Hands = new(2);
    public Container<Item> Bag = new(20);
    public Container<Ability> Spellbook = new(5);

    public void AddEquippable(Equippable equippable, int quantity = 1)
    {
        if (equippable is Weapon weapon)
        {
            Hands.AddItem(weapon, quantity);
        }
        else if (equippable is Item item)
        {
            Bag.AddItem(item, quantity);
        }
        else
        {
            Debug.LogError("Invalid equippable type: " + equippable.Name);
        }
    }
}
