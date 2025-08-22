using InventorySystem;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public Container<Weapon> Hands = new(2);
    public Container<Item> Bag = new(20);
    public Container<Ability> Spellbook = new(5);

    public void AddEquippable<T>(T equippable, Container<T> container, int quantity = 1) where T : InventoryObject
    {
        container.AddItem(equippable, quantity);
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
            AddEquippable(newWeapon, Hands);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Item newItem = Item.NewItem("Health Potion", "Restores health.", null, 5);
            AddEquippable(newItem, Bag);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Ability newAbility = Ability.NewAbility("Fireball", "Casts a fireball.", null, 20);
            AddEquippable(newAbility, Spellbook);
        }
    }
}
