using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private readonly UnityEvent _onGameOver = new UnityEvent();
    private readonly UnityEvent _onGameRestart = new UnityEvent();
    private readonly UnityEvent _onPlayerRespawn = new UnityEvent();
    private readonly UnityEvent _onCollectablePicked = new UnityEvent();
    private readonly UnityEvent<int> _onHpAmountChanged = new UnityEvent<int>();
    private readonly UnityEvent<int> _onLifeAmountChanged = new UnityEvent<int>();

    [SerializeField] private PlayerController player;

    // PARA BORRAR
    public bool canWin = false;
    [SerializeField] private GameObject wonScreen;
    
    private bool _gameOver = false;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
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
        _gameOver = true;
        Time.timeScale = 0;
    }

    // PARA CAMBIAR DESPUÉS
    public void RestartGame()
    {
        _onGameRestart?.Invoke();
    }

    public void RevivePlayer()
    {
        GetPlayer.GetPlayerRevived?.Invoke();
        RestartGameEvent();
    }

    private void RestartGameEvent()
    {
        Time.timeScale = 1;
        // PARA BORRAR
        wonScreen.SetActive(false);
        _gameOver = false;
        canWin = false;
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #region Events

    public UnityEvent GetGameOverEvent => _onGameOver;
    public UnityEvent GetGameRestarted => _onGameRestart;
    public UnityEvent GetPlayerRespawn => _onPlayerRespawn;
    public UnityEvent GetCollectablePicked => _onCollectablePicked;
    public UnityEvent<int> GetHpAmountChanged => _onHpAmountChanged;
    public UnityEvent<int> GetLifeAmountChanged => _onLifeAmountChanged;
    

    #endregion
    
    public PlayerController GetPlayer => player;
    
    public bool GetGameOver => _gameOver;
}
