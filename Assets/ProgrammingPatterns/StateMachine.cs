using UnityEngine;

public interface IState
{
    void Enter();
    void Update();
    void Exit();
}

public class StateMachine : MonoBehaviour
{
    public class IdleState : IState
    {
        void IState.Enter()
        {
            Debug.Log("Entering Idle State");
        }

        void IState.Update()
        {
            // Update logic for idle state
        }

        void IState.Exit()
        {
            Debug.Log("Exiting Idle State");
        }
    }

    public class WalkState : IState
    {
        void IState.Enter()
        {
            Debug.Log("Entering Walk State");
        }

        void IState.Update()
        {
            // Update logic for walk state
        }

        void IState.Exit()
        {
            Debug.Log("Exiting Walk State");
        }
    }

    private IState _currentState;

    public void ChangeState(IState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
        }

        _currentState = newState;

        if (_currentState != null)
        {
            _currentState.Enter();
        }
    }

    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.Update();
        }
    }
}
