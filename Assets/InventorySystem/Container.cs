namespace InventorySystem
{
    using System.Collections.Generic;

    public class Container<T> where T : Equippable
    {
        public List<T> Equippables { get; private set; } = new List<T>();
        public readonly int Capacity;

        public Container(int capacity)
        {
            Capacity = capacity;
        }

        public void AddItem(T item)
        {
            if (!Equippables.Contains(item) && Equippables.Count < Capacity)
            {
                if (item is IStackable stackableItem)
                {
                    var existingItem = Equippables.Find(i => i == item);
                    if (existingItem != null && existingItem is IStackable existingStackable)
                    {
                        // Stack the item
                        existingStackable.StackCount += stackableItem.StackCount;
                    }
                    else
                    {
                        // Add new item
                        Equippables.Add(item);
                    }
                }
                else
                {
                    Equippables.Add(item);
                }
            }
        }

        public void RemoveItem(T item)
        {
            if (Equippables.Contains(item))
            {
                if (item is IStackable stackableItem)
                {
                    var existingItem = Equippables.Find(i => i == item);
                    if (existingItem != null && existingItem is IStackable existingStackable)
                    {
                        existingStackable.StackCount -= stackableItem.StackCount;
                        if (existingStackable.StackCount <= 0)
                        {
                            // Removed last item from stack
                            Equippables.Remove((T)existingStackable);
                        }
                    }
                }
                else
                {
                    Equippables.Remove(item);
                }
            }
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
