using System.Collections.Generic;

public class Inventory : Singleton<Inventory>
{
    public InventorySystem.Container<InventorySystem.Equippable> EquippableContainer = new(100);
    public InventorySystem.Container<InventorySystem.Weapon> Hands = new(2);
    public InventorySystem.Container<InventorySystem.Item> ItemContainer = new(200);

    public void AddItem(InventorySystem.Equippable item)
    {
        // TODO: Implement adding item to the appropriate container
    }
}
