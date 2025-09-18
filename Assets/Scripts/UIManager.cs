using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] private Transform livesContainer;
    
    private void Start()
    {
        GameManager.Instance.GetGameOverEvent.AddListener(ShowGameOverScreen);
        GameManager.Instance.GetGameRestarted.AddListener(ResetUI);
        GameManager.Instance.GetLifeAmountChanged.AddListener(RemoveLife);
    }

    private void ShowGameOverScreen()
    {
        gameOverPanel.SetActive(true);
    }

    private void ResetUI()
    {
        gameOverPanel.SetActive(false);
        foreach (Transform child in livesContainer)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void RemoveLife(int currentHp)
    {
        foreach (Transform child in livesContainer)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in livesContainer)
        {
            if (currentHp > 0)
            {
                child.gameObject.SetActive(true);
                currentHp--;
            }
        }
    }
}
