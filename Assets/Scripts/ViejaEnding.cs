using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ViejaEnding : MonoBehaviour
{
    [SerializeField] private Image fadeToBlack;
    [SerializeField] private AudioSource rain;

    [SerializeField] private GameObject viejaEnding;
    [SerializeField] private GameObject viejaGoodEnding;
    [SerializeField] private GameObject vieja1;
    [SerializeField] private GameObject vieja2;
    [SerializeField] private GameObject vieja3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        fadeToBlack.color = Color.black;
        rain.volume = 0f;
        if (viejaEnding) viejaEnding.GetComponent<BoxCollider2D>().enabled = false;
        if (vieja1) vieja1.GetComponent<BoxCollider2D>().enabled = false;
        if (vieja2) vieja2.GetComponent<BoxCollider2D>().enabled = false;
        if (vieja3) vieja3.GetComponent<BoxCollider2D>().enabled = false;
        if (viejaGoodEnding) viejaGoodEnding.GetComponent<BoxCollider2D>().enabled = false;
        AudioManager.Instance.PlayClip(AudioManager.AudioList.ViejaDeath);
        GameManager.Instance.GetPlayer.StopInputs();
        GameManager.Instance.GetUIManager.inMenu = true;
        EndGame();
    }
    
    private void EndGame()
    {
        GameManager.Instance.GetUIManager.ShowBadEndingPanel();
        DOVirtual.DelayedCall(
            5f, () =>
            {
                AudioManager.Instance.PlayMusic(AudioManager.AudioList.SadEndingMelody);
                fadeToBlack.DOFade(0, 3f);
                DOVirtual.DelayedCall(12f, () => fadeToBlack.DOFade(1, 3f).OnComplete(BackToMenu));
            });
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
