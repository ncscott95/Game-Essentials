namespace InventorySystem
{
    public interface IStackable
    {
        int StackCount { get; set; }
        int MaxStackSize { get; }
    }
}
