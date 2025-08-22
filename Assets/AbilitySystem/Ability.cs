namespace AbilitySystem
{
    using UnityEngine;

    public class Ability
    {
        public readonly float Cooldown = 5f;

        public void Initialize(int index) { PlayerControllerBase.Instance.AbilityManager.RegisterAbility(this, index); }
        public void Deinitialize(int index) { PlayerControllerBase.Instance.AbilityManager.UnRegisterAbility(this, index); }

        public void Activate()
        {
            // Logic to activate the ability
        }
    }
}
