using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ViejaEnding : MonoBehaviour
{
    [SerializeField] private Image fade;
    [SerializeField] private AudioSource bossMusic;
    [SerializeField] private AudioSource rain;

    private void Start()
    {
        bossMusic.Play();
        rain.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        fade.color = Color.black;
        // AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerAttack);
        // AudioManager.Instance.StopClip();
        bossMusic.Stop();
        // rain.Stop();
        AudioManager.Instance.PlayClip(AudioManager.AudioList.ViejaDeath);
        Invoke(nameof(BackToMenu), 8f);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
