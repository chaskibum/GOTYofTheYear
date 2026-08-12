using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Slider slider;
        [SerializeField] private Button closeButton;
        [SerializeField] private Image fade;
        [SerializeField] private InputActionReference moveAction;
    
        private void Start()
        {
            HighlightPlayButton();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (moveAction.action.triggered)
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
        
        public void HighlightCloseButton()
        {
            closeButton.Select();
        }

        public void HighlightVolumeSlider()
        {
            print("highlightinf!");
            slider.Select();
        }
    }
}
