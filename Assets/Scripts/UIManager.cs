using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private Transform livesContainer;
    
    private void Start()
    {
        GameManager.Instance.GetGameOverEvent.AddListener(ShowGameOverScreen);
        GameManager.Instance.GetGameRestarted.AddListener(ResetUI);
        GameManager.Instance.GetLifeAmountChanged.AddListener(UpdateLife);
    }

    private void ShowGameOverScreen()
    {
        gameOverPanel.SetActive(true);
    }

    private void ResetUI()
    {
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        
        int baseLives = GameManager.Instance.GetPlayer.GetPlayerData.baseLives;

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
        }
    }
}
