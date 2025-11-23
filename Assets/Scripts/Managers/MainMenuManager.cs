using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Slider slider;
    
        private void Start()
        {
            HighlightPlayButton();
        }
    
        public void Play()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("GameLevel");
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void HighlightPlayButton()
        {
            playButton.Select();
        }

        public void HighlightVolumeSlider()
        {
            slider.Select();
        }
    }
}
