using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameLevel");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
