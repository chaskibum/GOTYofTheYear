using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
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
            
            /*Locale newLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            /*if (language == 1)
                newLocale = LocalizationSettings.AvailableLocales.GetLocale("en");#1#
            
            print(newLocale);
            LocalizationSettings.SelectedLocale = newLocale;*/
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
