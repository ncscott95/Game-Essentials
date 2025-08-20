using InventorySystem;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public Container<Weapon> Hands = new(2);
    public Container<Item> ItemContainer = new(200);

    public void AddEquippable(Equippable equippable, int quantity = 1)
    {
        if (equippable is Weapon weapon)
        {
            Hands.AddItem(weapon, quantity);
        }
        else if (equippable is Item item)
        {
            ItemContainer.AddItem(item, quantity);
        }
        else
        {
            Debug.LogError("Invalid equippable type: " + equippable.Name);
        }
    }
}
