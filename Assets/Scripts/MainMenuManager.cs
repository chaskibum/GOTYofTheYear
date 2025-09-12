using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("PrototypeScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
