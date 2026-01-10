using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ViejaEnding : MonoBehaviour
{
    [SerializeField] private Image fade;
    [SerializeField] private AudioSource rain;

    private void Start()
    {
        rain.volume = 0.8f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        fade.color = Color.black;
        // AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerAttack);
        // AudioManager.Instance.StopClip();
        rain.volume = -0.5f;
        rain.DOFade(1f, 8f);
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
