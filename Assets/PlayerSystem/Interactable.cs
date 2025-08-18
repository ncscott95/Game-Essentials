using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void OnInteract();
    public string InteractPrompt;
    public bool DestroyOnInteract;
}
