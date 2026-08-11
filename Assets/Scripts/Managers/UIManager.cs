using PlayerScripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;

        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject reviveButton;
        [SerializeField] private GameObject optionsButton;
        [SerializeField] private TMP_Dropdown languageDropdown;
        [SerializeField] private GameObject uSurePanel;
        [SerializeField] private Slider slider;
        [SerializeField] private Button button;
        [SerializeField] public GameObject pauseMenuUI;
        [SerializeField] private Transform livesContainer;
        [SerializeField] private GameObject bossHealthBars;

        [SerializeField] private GameObject cooldownBar;
        private Slider _cooldownBarSlider;

        private bool _isPaused = false;
        public bool inMenu = false;

        private PlayerController _player;

        private void Start()
        {
            AddListeners(true);
            if (PlayerPrefs.GetString("ImmunityUnlocked") == "Immunity") ShowCooldown();

            _player = GameManager.Instance.GetPlayer;
            _cooldownBarSlider = cooldownBar.GetComponentInChildren<Slider>();
            ChangeCooldownSliderMin();

            languageDropdown.value = LocalizationSettings.SelectedLocale.Identifier.Code == "es" ? 0 : 1;

            int lives = _player.GetPlayerLives;

            UpdateLife(lives);
        }

        private void OnDestroy()
        {
            AddListeners(false);
        }

        private void AddListeners(bool add)
        {
            _player = GameManager.Instance.GetPlayer;

            if (add)
            {
                GameManager.Instance.GetGameOverEvent.AddListener(ShowGameOverScreen);
                GameManager.Instance.GetGameRestarted.AddListener(RestartUI);
                GameManager.Instance.GetPlayer.GetPlayerRevived.AddListener(ResetUI);
                GameManager.Instance.GetLifeAmountChanged.AddListener(UpdateLife);
                GameManager.Instance.GetImmunityUnlocked?.AddListener(ShowCooldown);
                languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
                _player.GetPlayerRevived?.AddListener(ResetUI);
                return;
            }

            GameManager.Instance.GetGameOverEvent.RemoveListener(ShowGameOverScreen);
            GameManager.Instance.GetGameRestarted.RemoveListener(RestartUI);
            GameManager.Instance.GetPlayer.GetPlayerRevived.RemoveListener(ResetUI);
            GameManager.Instance.GetLifeAmountChanged.RemoveListener(UpdateLife);
            GameManager.Instance.GetImmunityUnlocked?.RemoveListener(ShowCooldown);
            languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
            _player.GetPlayerRevived?.RemoveListener(ResetUI);
        }

        private void Update()
        {
            if (inMenu) return;

            _cooldownBarSlider.value = _player.GetCooldown * -1;
        }

        public void OnPause(InputAction.CallbackContext ctx)
        {
            if (inMenu) return;
            if (ctx.performed && !uSurePanel.activeInHierarchy) HandlePause();
        }

        private void ShowGameOverScreen()
        {
            gameOverPanel.SetActive(true);
            optionsButton.SetActive(false);
        }

        private void ShowCooldown()
        {
            cooldownBar.SetActive(true);
        }

        private void ResetUI()
        {
            gameOverPanel.SetActive(false);
            reviveButton.SetActive(false);
            optionsButton.SetActive(true);
            if (PlayerPrefs.GetString("ImmunityUnlocked") == "Immunity") ShowCooldown();
            else cooldownBar.SetActive(false);

            if (pauseMenuUI.activeInHierarchy) HandlePause();

            int baseLives = _player.GetPlayerData.baseLives;

            UpdateLife(baseLives);
        }

        private void ResetBossHealthBars()
        {
            bossHealthBars.SetActive(true);
            foreach (Transform healthBar in bossHealthBars.transform)
            {
                healthBar.gameObject.SetActive(false);
            }
        }

        public void OnLanguageChanged(int language)
        {
            Locale newLocale = LocalizationSettings.AvailableLocales.GetLocale("es");
            if (language == 1)
                newLocale = LocalizationSettings.AvailableLocales.GetLocale("en");

            print(newLocale);
            LocalizationSettings.SelectedLocale = newLocale;
        }

        private void RestartUI()
        {
            gameOverPanel.SetActive(false);
            reviveButton.SetActive(false);
            optionsButton.SetActive(true);
            cooldownBar.SetActive(false);
            ChangeCooldownSliderMin(true);

            HandlePause();

            int baseLives = _player.GetPlayerData.baseLives;

            UpdateLife(baseLives);
        }

        private void UpdateLife(int currentHp)
        {
            foreach (Transform child in livesContainer)
            {
                child.gameObject.SetActive(false);
            }

            if (currentHp > 0)
            {
                foreach (Transform child in livesContainer)
                {
                    if (currentHp > 0)
                    {
                        child.gameObject.SetActive(true);
                        currentHp--;
                    }
                }
            }
            else
            {
                GameManager.Instance.GetGameOverEvent?.Invoke();
                reviveButton.SetActive(true);
                reviveButton.GetComponent<Button>().Select();
                optionsButton.SetActive(false);
            }
        }

        public void HandlePause()
        {
            _isPaused = !_isPaused;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (_isPaused)
            {
                playerInput.SwitchCurrentActionMap("UI");
                Time.timeScale = 0f;
                pauseMenuUI.SetActive(true);
                HighlightSliderButton();
                optionsButton.SetActive(false);
                bossHealthBars.SetActive(false);
            }
            else
            {
                playerInput.SwitchCurrentActionMap("Gameplay");
                Time.timeScale = 1f;
                pauseMenuUI.SetActive(false);
                optionsButton.SetActive(true);
                bossHealthBars.SetActive(true);
            }
        }

        public void SetInMenu(bool on = true)
        {
            if (on)
            {
                inMenu = true;
                playerInput.SwitchCurrentActionMap("UI");
                print(playerInput.currentActionMap);
            }
            else
            {
                inMenu = false;
                playerInput.SwitchCurrentActionMap("Gameplay");
                print(playerInput.currentActionMap);
            }
        }

        public void HighlightSureButton()
        {
            button.Select();
        }

        public void HighlightSliderButton()
        {
            slider.Select();
        }

        public void ChangeCooldownSliderMin(bool reset = false)
        {
            if (reset)
                _cooldownBarSlider.minValue = _player.GetPlayerData.baseShieldCooldown * -1;
            else
                _cooldownBarSlider.minValue = _player.GetPlayerData.shieldCooldown * -1;
        }
    }
}
