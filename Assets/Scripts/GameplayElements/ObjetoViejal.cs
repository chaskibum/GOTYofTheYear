using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Utils;

namespace GameplayElements
{
    public class ObjetoViejal : MonoBehaviour, IInteractable
    {
        private SpriteRenderer _sprite;
        private BoxCollider2D _collider;

        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject objectLight;
        private bool _isSpanish;
        private TextMeshProUGUI _interactPromptText;

        private bool _inRange;
        private bool _justClosed;
        private UIManager _uiManager;

        [SerializeField] private bool Viejal1 = false;
        [SerializeField] private bool Viejal2 = false;
        [SerializeField] private bool Viejal3 = false;

        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            _uiManager = GameManager.Instance.GetUIManager;

            _interactPromptText = interactPrompt.GetComponentInChildren<TextMeshProUGUI>();
            _interactPromptText.text = GetInteractionPrompt();
            LocalizationSettings.SelectedLocaleChanged += LanguageChanged;
            LanguageChanged(LocalizationSettings.SelectedLocale);

            if (PlayerPrefs.GetString("Viejal1") == name)
            {
                HideObject();
            }
            else if (PlayerPrefs.GetString("Viejal2") == name)
            {
                HideObject();
            }
            else if (PlayerPrefs.GetString("Viejal3") == name)
            {
                HideObject();
            }

            GameManager.Instance.GetGameRestarted?.AddListener(ResetCollectable);
        }

        public string GetInteractionPrompt()
        {
            if (_isSpanish)
            {
                if (GameManager.Instance.GetControllerConnected)
                {
                    return "Presiona <color=yellow>(Y)</color> para ver objeto especial";
                }
                else
                {
                    return "Presiona [E] para ver objeto especial";
                }
            }
            else
            {
                if (GameManager.Instance.GetControllerConnected)
                {
                    return "Press <color=yellow>(Y)</color> to view special item";
                }
                else
                {
                    return "Press [E] to view special item";
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

        private void HideObject()
        {
            _sprite.enabled = false;
            _collider.enabled = false;
            objectLight.SetActive(false);
        }

        private void OnDestroy()
        {

            GameManager.Instance.GetGameRestarted?.RemoveListener(ResetCollectable);
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
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (_inRange)
            {
                if (ctx.performed) Interact();
            }
        }

        public void Interact()
        {
            /*if (!panel.activeInHierarchy)
            {
                _uiManager.SetInMenu();
                panel.SetActive(true);
                Time.timeScale = 0;
                closeButton.Select();
            }
            else
            {
                ClosePanel();
            }*/
            _uiManager.SetInMenu();
            panel.SetActive(true);
            Time.timeScale = 0;
            closeButton.Select();
        }

        public void ClosePanel()
        {
            Time.timeScale = 1;
            panel.SetActive(false);
            _justClosed = true;
            _uiManager.SetInMenu(false);

            HideObject();

            if (Viejal1)
            {
                PlayerPrefs.SetString("Viejal1", name);
                print("viejal 1 saved");
            }
            else if (Viejal2)
            {
                PlayerPrefs.SetString("Viejal2", name);
                print("viejal 2 saved");
            }
            else if (Viejal3)
            {
                PlayerPrefs.SetString("Viejal3", name);
                print("viejal 3 saved");
            }
        }

        private void ResetCollectable()
        {
            _sprite.enabled = true;
            _collider.enabled = true;
            objectLight.SetActive(true);
            PlayerPrefs.SetString("Viejal1", "");
            PlayerPrefs.SetString("Viejal2", "");
            PlayerPrefs.SetString("Viejal3", "");
        }
    }
}
