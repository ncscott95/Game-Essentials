using InventorySystem;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public Container<Weapon> Hands = new(2);
    public Container<Item> Bag = new(20);
    public AbilityContainer Spellbook = new(2);

    public void AddToContainer<T>(T inventoryObject, Container<T> container, int quantity = 1) where T : InventoryObject
    {
        container.AddItem(inventoryObject, quantity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Inventory Contents:");
            Debug.Log($"Hands: {Hands}");
            Debug.Log($"Bag: {Bag}");
            Debug.Log($"Spellbook: {Spellbook}");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Weapon newWeapon = Weapon.NewWeapon("Sword", "A sharp blade.", null, 10);
            AddToContainer(newWeapon, Hands);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Item newItem = Item.NewItem("Health Potion", "Restores health.", null, 5);
            AddToContainer(newItem, Bag);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Ability newAbility = Ability.NewAbility("Fireball", "Casts a fireball.", null);
            AddToContainer(newAbility, Spellbook);
        }
    }
}
