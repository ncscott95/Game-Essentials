using System.Collections.Generic;
using UnityEngine;

public class Conversation : ScriptableObject
{
    public List<DialogueElement> elements;
    public string DialogueName;
}

[System.Serializable]
public class DialogueElement
{
    public string Speaker;
    public string Line;
    public string Sound;

    public DialogueElement(string speaker, string line, string sound)
    {
        Speaker = speaker;
        Line = line;
        Sound = sound;
    }
}
