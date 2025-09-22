using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Alpha Test Level 1");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
