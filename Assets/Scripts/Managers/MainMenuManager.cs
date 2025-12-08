using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private Image fade;
    
        private void Start()
        {
            HighlightPlayButton();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            /*Locale newLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            /*if (language == 1)
                newLocale = LocalizationSettings.AvailableLocales.GetLocale("en");#1#

            print(newLocale);
            LocalizationSettings.SelectedLocale = newLocale;*/
        }

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (Input.GetButtonDown("Vertical"))
                {
                    playButton.Select();
                }
            }
        }

        public void Play()
        {
            Time.timeScale = 1;
            
            fade.DOFade(1f, 2f).OnComplete(() => SceneManager.LoadScene("GameLevel"));
        }

        public void Exit()
        {
            print("exiting...");
            fade.DOFade(1f, 1f).OnComplete(() => Application.Quit());
            print("done");
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
