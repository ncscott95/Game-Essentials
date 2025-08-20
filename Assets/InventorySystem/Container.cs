namespace InventorySystem
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Container<T> where T : Equippable
    {
        public List<T> Equippables { get; private set; } = new List<T>();
        public readonly int Capacity;

        public Container(int capacity)
        {
            Capacity = capacity;
        }

        public bool AddItem(T item, int quantity = 1)
        {
            if (item is IStackable)
            {
                return AddStackableItem(item, quantity);
            }
            else
            {
                // You should never need to add multiple non-stackable items at once
                return AddSingleItem(item);
            }
        }

        // Add a single, non-stackable item
        private bool AddSingleItem(T item)
        {
            if (Equippables.Count >= Capacity)
            {
                Debug.LogWarning("Container is full. Cannot add item: " + item.Name);
                return false;
            }

            Equippables.Add(item);
            return true;
        }

        // Add a stackable item with a specific quantity
        private bool AddStackableItem(T item, int quantity)
        {
            IStackable stackableItem = item as IStackable;

            // Find an existing stack of this item.
            T existingItem = Equippables.Find(i => i.Name == item.Name);

            if (existingItem != null && existingItem is IStackable existingStackable)
            {
                // Calculate how much room is left in the stack.
                int stackRoom = existingStackable.MaxStackSize - existingStackable.StackCount;
                int quantityToAdd = Mathf.Min(quantity, stackRoom);

                if (quantityToAdd > 0)
                {
                    existingStackable.StackCount += quantityToAdd;
                    return true;
                }
                else
                {
                    Debug.Log("Stack is full. Cannot add more of this item.");
                    return false;
                }
            }
            else
            {
                // Check for capacity before adding a new item.
                if (Equippables.Count >= Capacity)
                {
                    Debug.LogWarning("Container is full. Cannot add item: " + item.Name);
                    return false;
                }

                // Add a new stackable item if one doesn't exist yet.
                stackableItem.StackCount = Mathf.Min(quantity, stackableItem.MaxStackSize);
                Equippables.Add(item);
                return true;
            }
        }

        public bool RemoveItem(T item, int quantity = 1)
        {
            if (item is IStackable)
            {
                return RemoveStackableItem(item, quantity);
            }
            else
            {
                // You should never need to remove multiple non-stackable items at once
                return RemoveSingleItem(item);
            }
        }

        // Remove a single, non-stackable item
        private bool RemoveSingleItem(T item)
        {
            return Equippables.Remove(item);
        }

        // Remove a stackable item with a specific quantity
        private bool RemoveStackableItem(T item, int quantity)
        {
            IStackable existingStackable = item as IStackable;
            if (Equippables.Contains(item))
            {
                if (existingStackable.StackCount > quantity)
                {
                    existingStackable.StackCount -= quantity;
                    return true;
                }
                else if (existingStackable.StackCount <= quantity)
                {
                    // Remove the entire stack if the quantity is greater than or equal to the current count.
                    return Equippables.Remove(item);
                }
            }

            Debug.LogWarning("Item not found in container or not enough items in the stack.");
            return false;
        }

        public void SwapItems(int indexA, int indexB)
        {
            if (indexA >= 0 && indexA < Equippables.Count && indexB >= 0 && indexB < Equippables.Count)
            {
                (Equippables[indexB], Equippables[indexA]) = (Equippables[indexA], Equippables[indexB]);
            }
        }
    }
}
