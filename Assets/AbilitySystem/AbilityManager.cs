using System.Collections;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private bool _canUseAbility;

    public void RegisterAbility(AbilitySystem.Ability ability, int index)
    {
        UnityEngine.InputSystem.InputAction action = GetAbilityInput(index);
        if (action == null)
        {
            Debug.LogWarning("No input action found for ability index: " + index);
            return;
        }
        action.performed += ctx => UseAbility(ability);
    }

    public void UnRegisterAbility(AbilitySystem.Ability ability, int index)
    {
        UnityEngine.InputSystem.InputAction action = GetAbilityInput(index);
        if (action == null)
        {
            Debug.LogWarning("No input action found for ability index: " + index);
            return;
        }
        action.performed -= ctx => UseAbility(ability);
    }

    private static UnityEngine.InputSystem.InputAction GetAbilityInput(int index)
    {
        // Populate with all of the player's possible ability keybinds
        return index switch
        {
            0 => PlayerControllerBase.Instance.Actions.Player.Ability1,
            1 => PlayerControllerBase.Instance.Actions.Player.Ability2,
            _ => null,
        };
    }

    private void UseAbility(AbilitySystem.Ability ability)
    {
        if (_canUseAbility)
        {
            ability.Activate();
            StartCoroutine(AbilityCooldownCoroutine(ability));
        }
    }

    private IEnumerator AbilityCooldownCoroutine(AbilitySystem.Ability ability)
    {
        _canUseAbility = false;
        yield return new WaitForSeconds(ability.Cooldown);
        _canUseAbility = true;
    }
}
