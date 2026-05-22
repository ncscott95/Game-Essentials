using UnityEngine;
using System.Collections.Generic;

public interface IObserver
{
    void OnNotify();
}

public class Subject : MonoBehaviour
{
    private List<IObserver> _observers = new List<IObserver>();

    public void RegisterObserver(IObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void UnregisterObserver(IObserver observer)
    {
        if (_observers.Contains(observer))
        {
            _observers.Remove(observer);
        }
    }

    public void NotifyObservers()
    {
        foreach (IObserver observer in _observers)
        {
            observer.OnNotify();
        }
    }
}
