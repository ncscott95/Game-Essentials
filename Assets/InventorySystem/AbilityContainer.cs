namespace InventorySystem
{
    public class AbilityContainer : Container<Ability>
    {
        public AbilityContainer(int capacity) : base(capacity)
        {

        }

        protected override void OnAddItem(Ability item, int index)
        {
            base.OnAddItem(item, index);
            item.AbilityScript.Initialize(index);
        }

        protected override void OnRemoveItem(Ability item, int index)
        {
            base.OnRemoveItem(item, index);
            item.AbilityScript.Deinitialize(index);
        }

        public override void SwapItems(int indexA, int indexB)
        {
            base.SwapItems(indexA, indexB);
            // Additional logic for swapping abilities
        }
    }
}
