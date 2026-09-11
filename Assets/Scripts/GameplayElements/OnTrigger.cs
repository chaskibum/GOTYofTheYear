using Managers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.UI;

public class OnTrigger : MonoBehaviour
{
    [SerializeField] private bool isAttackPanel;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private Button attackPanelCloseButton;
    [SerializeField] private Button ghostPanelCloseButton;

    [SerializeField] private GameObject attackPanel;
    [SerializeField] private GameObject ghostPanel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttackPanel)
        {
            PlayerController playerController = other.transform.GetComponent<PlayerController>();

            if (!playerController.hasAttacked)
            {
                Time.timeScale = 0;
                attackPanelCloseButton.Select();
                attackPanel.SetActive(true);
                uiManager.SetInMenu();
            }
        }
        else
        {
            if (!uiManager.shownGhostTutorialPanel)
            {
                Time.timeScale = 0;
                ghostPanelCloseButton.Select();
                ghostPanel.SetActive(true);
                uiManager.shownGhostTutorialPanel = true;
                uiManager.SetInMenu();
            }
        }
    }

    public void CloseAttackPanel()
    {
        Time.timeScale = 1;
        attackPanel.SetActive(false);
        uiManager.SetInMenu(false);
    }

    public void CloseGhostPanel()
    {
        Time.timeScale = 1;
        ghostPanel.SetActive(false);
        uiManager.SetInMenu(false);
    }
}
