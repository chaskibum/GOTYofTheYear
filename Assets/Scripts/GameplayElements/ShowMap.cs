using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace GameplayElements
{
    public class ShowMap : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject map;
        [SerializeField] private GameObject interactPrompt;
        private TextMeshProUGUI _interactPromptText;

        [SerializeField] private Button closeButton;
        private bool _isSpanish;

        private bool _inRange;
        private bool _justClosed;
        private UIManager _uiManager;

        private void Start()
        {
            _interactPromptText = interactPrompt.GetComponentInChildren<TextMeshProUGUI>();
            _interactPromptText.text = GetInteractionPrompt();
            LocalizationSettings.SelectedLocaleChanged += LanguageChanged;
            LanguageChanged(LocalizationSettings.SelectedLocale);

            _uiManager = GameManager.Instance.GetUIManager;
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= LanguageChanged;
        }

        public string GetInteractionPrompt()
        {
            if (_isSpanish)
            {
                if (GameManager.Instance.GetControllerConnected)
                {
                    return "Presiona <color=yellow>(Y)</color> para ver mapa";
                }
                else
                {
                    return "Presiona [E] para ver mapa";
                }
            }
            else
            {
                if (GameManager.Instance.GetControllerConnected)
                {
                    return "Press <color=yellow>(Y)</color> to view map";
                }
                else
                {
                    return "Press [E] to view map";
                }
            }
        }

        private void LanguageChanged(Locale lang)
        {
            if (lang.ToString() != "Spanish (es)")
            {
                _isSpanish = false;
            }
            else
            {
                _isSpanish = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            interactPrompt.SetActive(true);
            _interactPromptText.text = GetInteractionPrompt();
            _inRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            interactPrompt.SetActive(false);
            map.SetActive(false);
            _inRange = false;
        }

        private void Update()
        {
            if (_justClosed)
            {
                _justClosed = false;
                return;
            }

            // if (_inRange) CheckInteractInput();
            // if (_inRange && map.activeInHierarchy) CheckCloseInput();
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (_inRange)
            {
                if (ctx.performed) Interact();
            }
        }

        /*private void CheckCloseInput()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                ClosePanel();
            }
        }*/

        public void Interact()
        {
            if (!map.activeInHierarchy)
            {
                _uiManager.SetInMenu();
                map.SetActive(true);
                Time.timeScale = 0;
                closeButton.Select();
            }
            else
            {
                ClosePanel();
            }
        }

        public void ClosePanel()
        {
            Time.timeScale = 1;
            map.SetActive(false);
            _justClosed = true;
            _uiManager.SetInMenu(false);
        }
    }
}
