using PlayerScripts;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject optionsButton;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Transform livesContainer;

    [SerializeField] private GameObject cooldownBar;
    private Slider _cooldownBarSlider;
    
    private bool _isPaused = false;

    private PlayerController _player;
    
    private void Start()
    {
        GameManager.Instance.GetGameOverEvent.AddListener(ShowGameOverScreen);
        GameManager.Instance.GetGameRestarted.AddListener(ResetUI);
        GameManager.Instance.GetLifeAmountChanged.AddListener(UpdateLife);

        _player = GameManager.Instance.GetPlayer;
        _player.GetPlayerRevived?.AddListener(ResetUI);
        _cooldownBarSlider = cooldownBar.GetComponentInChildren<Slider>();
        
        int lives = _player.GetPlayerLives;

        UpdateLife(lives);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HandlePause();

        _cooldownBarSlider.value = _player.GetCooldown * -1;
    }

    private void ShowGameOverScreen()
    {
        gameOverPanel.SetActive(true);
        optionsButton.SetActive(false);
    }

    private void ResetUI()
    {
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        optionsButton.SetActive(true);
        
        if (pauseMenuUI.activeInHierarchy) HandlePause();
        
        int baseLives = _player.GetPlayerData.baseLives;

        UpdateLife(baseLives);
    }

    private void UpdateLife(int currentHp)
    {
        foreach (Transform child in livesContainer)
        {
            child.gameObject.SetActive(false);
        }

        if (currentHp > 0)
        {
            foreach (Transform child in livesContainer)
            {
                if (currentHp > 0)
                {
                    child.gameObject.SetActive(true);
                    currentHp--;
                }
            }
        }
        else
        {
            GameManager.Instance.GetGameOverEvent?.Invoke();
            restartButton.SetActive(true);
            optionsButton.SetActive(false);
        }
    }
    
    public void HandlePause()
    {
        _isPaused = !_isPaused;
    
        if (_isPaused)
        {
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
        }
    }
}
