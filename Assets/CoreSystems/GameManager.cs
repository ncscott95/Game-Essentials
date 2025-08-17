using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Walking,
        Combat,
        Dialogue,
        Menu,
        Dead,
        Cutscene,
    }

    public GameState CurrentState { get; private set; }
    [SerializeField] private GameState _initialState;
    [SerializeField] private GameObject _deathScreen;

    public override void Awake()
    {
        base.Awake();

        SetState(_initialState);
        _deathScreen.SetActive(false);
    }

    public void SetState(GameState state)
    {
        // Make cursor usable only if in menu state
        bool isMenu = state == GameState.Menu || state == GameState.Dead;
        Cursor.visible = isMenu;
        Cursor.lockState = isMenu ? CursorLockMode.None : CursorLockMode.Locked;

        Instance.CurrentState = state;
    }

    public bool IsInState(GameState state)
    {
        return CurrentState == state;
    }

    public void OnDeath()
    {
        Instance.SetState(GameState.Dead);

        FadeManager.Instance.CloseEyesAndDo(() =>
        {
            _deathScreen.SetActive(true);
        });
    }
}
