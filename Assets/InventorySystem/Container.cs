namespace InventorySystem
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Container<T> where T : InventoryObject
    {
        public List<T> Objects { get; private set; } = new List<T>();
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
            if (Objects.Count >= Capacity)
            {
                Debug.LogWarning("Container is full. Cannot add item: " + item.Name);
                return false;
            }

            Objects.Add(item);
            OnAddItem(item, Objects.Count - 1);
            return true;
        }

        // Add a stackable item with a specific quantity
        private bool AddStackableItem(T item, int quantity)
        {
            IStackable stackableItem = item as IStackable;

            // Find an existing stack of this item.
            T existingItem = Objects.Find(i => i.Name == item.Name);

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
                if (Objects.Count >= Capacity)
                {
                    Debug.LogWarning("Container is full. Cannot add item: " + item.Name);
                    return false;
                }

                // Add a new stackable item if one doesn't exist yet.
                stackableItem.StackCount = Mathf.Min(quantity, stackableItem.MaxStackSize);
                Objects.Add(item);
                OnAddItem(item, Objects.Count - 1);
                return true;
            }
        }

        protected virtual void OnAddItem(T item, int index)
        {
            Debug.Log("Item added to container: " + item.Name + " at index: " + index);
        }

        public bool RemoveItem(T item, int quantity = 1, bool removeAll = false)
        {
            if (item is IStackable && !removeAll)
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
            if (Objects.Contains(item))
            {
                int index = Objects.IndexOf(item);
                Objects.Remove(item);
                OnRemoveItem(item, index);
                return true;
            }

            Debug.LogWarning("Item not found in container.");
            return false;
        }

        // Remove a stackable item with a specific quantity
        private bool RemoveStackableItem(T item, int quantity)
        {
            IStackable existingStackable = item as IStackable;
            if (Objects.Contains(item))
            {
                if (existingStackable.StackCount > quantity)
                {
                    existingStackable.StackCount -= quantity;
                    return true;
                }
                else if (existingStackable.StackCount <= quantity)
                {
                    // Remove the entire stack if the quantity is greater than or equal to the current count.
                    int index = Objects.IndexOf(item);
                    Objects.Remove(item);
                    OnRemoveItem(item, index);
                    return true;
                }
            }

            Debug.LogWarning("Item not found in container or not enough items in the stack.");
            return false;
        }

        protected virtual void OnRemoveItem(T item, int index)
        {
            Debug.Log("Item removed from container: " + item.Name + " at index: " + index);
        }

        public virtual void SwapItems(int indexA, int indexB)
        {
            if (indexA >= 0 && indexA < Objects.Count && indexB >= 0 && indexB < Objects.Count)
            {
                T tempA = Objects[indexA];
                T tempB = Objects[indexB];

                RemoveItem(Objects[indexA]);
                RemoveItem(Objects[indexB]);

                AddItem(tempA, indexB);
                AddItem(tempB, indexA);
            }
        }
    }
}
