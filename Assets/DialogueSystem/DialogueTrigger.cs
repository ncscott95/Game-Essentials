using UnityEngine;

public class DialogueTrigger : Trigger
{
    public string DialogueName;

    public override void OnEnter(Collider other)
    {
        DialogueManager.Instance.PlayDialogue(DialogueName);
    }
}
