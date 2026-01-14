using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ViejaEnding : MonoBehaviour
{
    [SerializeField] private Image fade;
    [SerializeField] private AudioSource rain;

    [SerializeField] private GameObject viejaEnding;
    [SerializeField] private GameObject viejaGoodEnding;
    [SerializeField] private GameObject vieja1;
    [SerializeField] private GameObject vieja2;
    [SerializeField] private GameObject vieja3;

    private void Start()
    {
        rain.volume = 0.8f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        fade.color = Color.black;
        rain.volume = 0f;
        rain.DOFade(1f, 8f);
        viejaEnding.GetComponent<BoxCollider2D>().enabled = false;
        vieja1.GetComponent<BoxCollider2D>().enabled = false;
        vieja2.GetComponent<BoxCollider2D>().enabled = false;
        vieja3.GetComponent<BoxCollider2D>().enabled = false;
        viejaGoodEnding.GetComponent<BoxCollider2D>().enabled = false;
        AudioManager.Instance.PlayClip(AudioManager.AudioList.ViejaDeath);
        GameManager.Instance.GetPlayer.StopInputs();
        GameManager.Instance.GetUIManager.inMenu = true;
        Invoke(nameof(BackToMenu), 8f);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
