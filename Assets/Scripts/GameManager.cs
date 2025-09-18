using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private readonly UnityEvent _onGameOver = new UnityEvent();
    private readonly UnityEvent _onGameRestart = new UnityEvent();
    private readonly UnityEvent<int> _onLifeAmountChanged = new UnityEvent<int>();

    [SerializeField] private PlayerController player;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _onGameOver.AddListener(GameOverScreen);
        _onGameRestart.AddListener(RestartGameEvent);
    }

    private void GameOverScreen()
    {
        Time.timeScale = 0;
    }

    // PARA CAMBIAR DESPUÉS
    public void RestartGame()
    {
        _onGameRestart?.Invoke();
    }

    private void RestartGameEvent()
    {
        Time.timeScale = 1;
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #region Events

    public UnityEvent GetGameOverEvent => _onGameOver;
    public UnityEvent GetGameRestarted => _onGameRestart;
    public UnityEvent<int> GetLifeAmountChanged => _onLifeAmountChanged;
    

    #endregion
    
    public PlayerController GetPlayer => player;
}
