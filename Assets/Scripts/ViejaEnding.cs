using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ViejaEnding : MonoBehaviour
{
    [SerializeField] private Image fade;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        fade.color = Color.black;
        // AudioManager.Instance.PlayClip(AudioManager.AudioList.PlayerAttack);
        AudioManager.Instance.PlayClip(AudioManager.AudioList.ViejaDeath);
        Invoke(nameof(BackToMenu), 5f);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
