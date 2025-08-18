using System.Collections.Generic;
using UnityEngine;

public class DialogueDemo : MonoBehaviour
{
    [SerializeField] private List<string> _dialogueNames;
    [SerializeField] private bool _loopFinalDialogue;
    private Queue<string> _nameQueue = new();
    private string _currentdialogue;

    void Start()
    {
        foreach (string convo in _dialogueNames) _nameQueue.Enqueue(convo);
        SetNextDialogue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DialogueManager.Instance.PlayDialogue(_currentdialogue);
            SetNextDialogue();
        }
    }

    private void SetNextDialogue()
    {
        if (_nameQueue.Count > 0) _currentdialogue = _nameQueue.Dequeue();
        else if (!_loopFinalDialogue) Destroy(this);
    }
}
