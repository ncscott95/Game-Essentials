using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private TextAsset _dialogueFile;
    [SerializeField] private DialoguePlayer _dialoguePlayer;
    private Dictionary<string, Conversation> _dialogueDictionary = new();
    private Dictionary<string, UnityAction> _onCompleteActions = new();

    public override void Awake()
    {
        base.Awake();
        if (_dialogueFile == null)
        {
            Debug.LogWarning("No dialogue file found!");
            return;
        }

        ParseDialogueFile();
    }

    public void PlayDialogue(string dialogueName)
    {
        if (!_dialogueDictionary.ContainsKey(dialogueName))
        {
            Debug.LogError($"Could not find dialogue {dialogueName}!");
            return;
        }

        _dialoguePlayer.PlayDialogue(_dialogueDictionary[dialogueName]);
    }

    public void RegisterOnCompleteAction(string conversationName, UnityAction action)
    {
        if (!_onCompleteActions.ContainsKey(conversationName))
        {
            _onCompleteActions[conversationName] = action;
        }
        else
        {
            _onCompleteActions[conversationName] += action;
        }
    }

    public UnityAction GetOnCompleteAction(string dialogueName)
    {
        if (!_onCompleteActions.ContainsKey(dialogueName))
        {
            Debug.LogWarning($"Could not find OnComplete for {dialogueName}!");
            return null;
        }

        return _onCompleteActions[dialogueName];
    }

    public void TryAdvanceDialogue()
    {
        _dialoguePlayer.TryAdvanceDialogue();
    }

    private void ParseDialogueFile()
    {
        string text = _dialogueFile.text;
        string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        Conversation currentConversation = null;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith("#"))
            {
                string conversationName = line.Substring(1);
                currentConversation = ScriptableObject.CreateInstance<Conversation>();
                currentConversation.DialogueName = conversationName;
                currentConversation.elements = new List<DialogueElement>();
                _dialogueDictionary[conversationName] = currentConversation;
            }
            else if (currentConversation != null)
            {
                string speaker = GetSpeakerName(line);
                string dialogue = (i + 1 < lines.Length) ? lines[++i] : "";
                string sound = "";
                if (i + 1 < lines.Length && lines[i + 1].StartsWith("("))
                {
                    sound = lines[++i];
                    sound = sound.Substring(1, sound.Length - 2);
                }
                currentConversation.elements.Add(new DialogueElement(speaker, dialogue, sound));
            }
        }
    }

    // Translates from screenplay label to in-game speaker name
    private string GetSpeakerName(string label)
    {
        return label switch
        {
            "NARRATION" => "",
            "PLAYER" => "You",
            _ => ToTitleCase(label)
        };
    }

    private string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        return textInfo.ToTitleCase(input.ToLower());
    }
    
    public void HideDialogue() { _dialoguePlayer.HideDialogue(); }
}
