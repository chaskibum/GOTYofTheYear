using System;
using PlayerScripts;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] private Transform livesContainer;
    
    private void Start()
    {
        GameManager.Instance.GetGameOverEvent.AddListener(() => ShowGameOverScreen());
        GameManager.Instance.GetGameRestarted.AddListener(() => ResetUI());
        GameManager.Instance.GetLifeAmountChanged.AddListener((hp) => RemoveLife(hp));
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
        if (currentHp <= 0)
        {
            foreach (Transform child in livesContainer)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
            livesContainer.GetChild(currentHp).gameObject.SetActive(false);
    }
}
