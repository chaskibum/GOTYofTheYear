using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private UnityEvent _onGameOver = new UnityEvent();
    private UnityEvent _onGameRestart = new UnityEvent();
    private UnityEvent<int> _onLifeAmountChanged = new UnityEvent<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _onGameOver.AddListener(() => GameOverScreen());
        _onGameRestart.AddListener(() => RestartGameEvent());
    }

    private void GameOverScreen()
    {
        Time.timeScale = 0;
    }

    // PA CAMBIAR DESPUES
    public void RestartGame()
    {
        _onGameRestart?.Invoke();
    }

    private void RestartGameEvent()
    {
        Time.timeScale = 1;
    }

    public UnityEvent GetGameOverEvent => _onGameOver;
    public UnityEvent GetGameRestarted => _onGameRestart;
    public UnityEvent<int> GetLifeAmountChanged => _onLifeAmountChanged;
}
