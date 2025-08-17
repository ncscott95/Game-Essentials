using System.Collections;
using TMPro;
using UnityEngine;

public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] private GameObject _dialogueWindow;
    [SerializeField] private TMP_Text _speakerText;
    [SerializeField] private TMP_Text _lineText;

    private Conversation conversation;
    private int dialogueIndex = -1;
    private DialogueElement currentElement;
    private GameManager.GameState oldState;

    private bool finishedTypewriter;
    private const float DIALOGUE_SPEED = 0.03f;

    public void HideDialogue()
    {
        _dialogueWindow.SetActive(false);
    }

    public void PlayDialogue(Conversation conversation)
    {
        this.conversation = conversation;
        _dialogueWindow.SetActive(true);

        oldState = GameManager.Instance.CurrentState;
        GameManager.Instance.SetState(GameManager.GameState.Dialogue);

        dialogueIndex = -1;
        AdvanceDialogue();
    }

    public void TryAdvanceDialogue()
    {
        if (!finishedTypewriter)
        {
            StopAllCoroutines();
            finishedTypewriter = true;
            _lineText.maxVisibleCharacters = currentElement.Line.Length;
        }
        else
        {
            if (dialogueIndex == conversation.elements.Count) return;
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        dialogueIndex++;

        if (dialogueIndex >= conversation.elements.Count)
        {
            _dialogueWindow.SetActive(false);
            GameManager.Instance.SetState(oldState);

            var completedConversation = conversation;
            DialogueManager.Instance.GetOnCompleteAction(completedConversation.DialogueName)?.Invoke();

            if (conversation == completedConversation)
            {
                conversation = null;
                dialogueIndex = -1;
            }
            return;
        }

        currentElement = conversation.elements[dialogueIndex];

        _speakerText.text = currentElement.Speaker;
        _lineText.text = currentElement.Line;
        // TODO: play sound if it exists
        StartCoroutine(TypewriterEffect());
    }

    private IEnumerator TypewriterEffect()
    {
        finishedTypewriter = false;
        int i = 0;
        _lineText.maxVisibleCharacters = i;
        int length = currentElement.Line.Length;

        while (i < length)
        {
            i++;
            _lineText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(DIALOGUE_SPEED);
        }

        finishedTypewriter = true;
    }
}
