using UnityEngine;

public class ViejaPrototipo : MonoBehaviour
{
    [SerializeField] private GameObject grannyDialog;
    
    // PARA BORRAR
    [SerializeField] private GameObject gameWonScreen;
    [SerializeField] private GameObject restartButton;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.canWin)
        {
            Time.timeScale = 0;
            gameWonScreen.SetActive(true);
            restartButton.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        grannyDialog.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        grannyDialog.SetActive(false);
    }
}
