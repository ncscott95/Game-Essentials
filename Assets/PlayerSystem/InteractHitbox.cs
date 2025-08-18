using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractHitbox : MonoBehaviour
{
    [SerializeField] private GameObject _interactPopup;
    [SerializeField] private TMP_Text _interactPrompt;
    private const int INTERACTABLE_LAYER_MASK = 1 << 10;
    private List<Transform> _availableInteractables = new List<Transform>();
    private Interactable _selected;

    void OnEnable()
    {
        _availableInteractables.Clear();
        _interactPopup.SetActive(false);
    }

    void Update()
    {
        _availableInteractables.RemoveAll(item => item == null);
        _availableInteractables.Sort((a, b) =>
            Vector3.SqrMagnitude(a.position - transform.position)
            .CompareTo(Vector3.SqrMagnitude(b.position - transform.position)));

        if (_availableInteractables.Count > 0 && GameManager.Instance.CurrentState != GameManager.GameState.Dialogue)
        {
            _selected = _availableInteractables[0].GetComponent<Interactable>();
            if (_selected == null)
            {
                _availableInteractables.RemoveAt(0);
                return;
            }
            _interactPrompt.text = _selected.InteractPrompt;
            _interactPopup.SetActive(true);
        }
        else
        {
            _selected = null;
            _interactPopup.SetActive(false);
            _interactPrompt.text = "";
        }
    }

    public void TryInteract()
    {
        if (_selected != null)
        {
            _selected.OnInteract();
            if (_selected.DestroyOnInteract)
            {
                _availableInteractables.RemoveAt(0);
                Destroy(_selected.gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & INTERACTABLE_LAYER_MASK) != 0 && other.TryGetComponent<Interactable>(out var hit))
        {
            _availableInteractables.Add(hit.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & INTERACTABLE_LAYER_MASK) != 0 && other.TryGetComponent<Interactable>(out var hit))
        {
            _availableInteractables.Remove(hit.transform);
        }
    }
}
