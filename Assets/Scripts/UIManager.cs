using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    
    private void Start()
    {
        GameManager.Instance.GetGameOverEvent.AddListener(() => ShowGameOverScreen());
        GameManager.Instance.GetGameRestarted.AddListener(() => HideGameOverScreen());
    }

    private void ShowGameOverScreen()
    {
        gameOverPanel.SetActive(true);
    }

    private void HideGameOverScreen()
    {
        gameOverPanel.SetActive(false);
    }
}
