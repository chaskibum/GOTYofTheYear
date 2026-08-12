using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace GameplayElements
{
    public class SaveSpotManager : MonoBehaviour, IInteractable
    {
        private Vector3 _lastSavedRespawnPosition;
        private Animator _animator;
        private static event Action<SaveSpotManager> OnCollisionEvent;

        [SerializeField] private bool isMainRespawn;
        [SerializeField] private AudioSource activatingSound;
        [SerializeField] private AudioSource torchSound;

        [SerializeField] private GameObject interactPrompt;
        private TextMeshProUGUI _interactPromptText;

        private bool _isInRange;
        private bool _isSpanish;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _interactPromptText = interactPrompt.GetComponentInChildren<TextMeshProUGUI>();
            _interactPromptText.text = GetInteractionPrompt();

            LocalizationSettings.SelectedLocaleChanged += LanguageChanged;
            LanguageChanged(LocalizationSettings.SelectedLocale);

            GameManager.Instance.GetGameRestarted?.AddListener(ResetAnimations);
            GameManager.Instance.GetPlayer.GetPlayerRevived?.AddListener(ResetAnimations);
            Invoke(nameof(ActivateRespawn), 0.1f);
        }

        public string GetInteractionPrompt()
        {
            if (_isSpanish)
            {
                if (GameManager.Instance.GetControllerConnected)
                {
                    return "Presiona <color=yellow>(Y)</color> para guardar";
                }
                else
                {
                    return "Presiona [E] para guardar";
                }
            }
            else
            {
                if (GameManager.Instance.GetControllerConnected)
                {
                    return "Press <color=yellow>(Y)</color> to save";
                }
                else
                {
                    return "Press [E] to save";
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

        private void ActivateRespawn()
        {
            if (_isInRange)
            {
                Interact();
            }
        }

        private void OnEnable()
        {
            OnCollisionEvent += HandleAnimations;
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= LanguageChanged;
            OnCollisionEvent -= HandleAnimations;
            GameManager.Instance.GetGameRestarted?.RemoveListener(ResetAnimations);
            GameManager.Instance.GetPlayer.GetPlayerRevived?.RemoveListener(ResetAnimations);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _isInRange = true;
            _interactPromptText.text = GetInteractionPrompt();
            print(_isSpanish);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _isInRange = false;
        }

        void Update()
        {
            if (_isInRange)
            {
                _animator.SetBool("OnRange", true);
            }
            else
            {
                _animator.SetBool("OnRange", false);
            }
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (_isInRange)
            {
                if (ctx.performed) Interact();
            }
        }
        public void Interact()
        {
            if (isMainRespawn) ActivateMainRespawn();
            else ActivateBonfireRespawn();

            OnCollisionEvent?.Invoke(this);
        }

        private void ActivateMainRespawn()
        {
            _lastSavedRespawnPosition = transform.position;
            GameManager.Instance.GetPlayer.SetMainRespawnPosition(_lastSavedRespawnPosition);
            PlayActivatingSound();
            GameManager.Instance.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
        }

        private void ActivateBonfireRespawn()
        {
            _lastSavedRespawnPosition = transform.position;
            GameManager.Instance.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
        }

        private void HandleAnimations(SaveSpotManager activated)
        {
            if (!_animator) return;

            bool isActive = activated == this;

            if (isMainRespawn && _animator.GetBool("Activated")) return;

            _animator.SetBool("Activated", isActive);
        }

        public void PlayActivatingSound()
        {
            if (!activatingSound) return;
            activatingSound.Play();
            if (torchSound) torchSound.Play();
        }

        private void ResetAnimations()
        {
            if (!_animator) return;

            _animator.SetBool("Activated", false);
            Invoke(nameof(ActivateRespawn), 0.1f);
        }
    }
}
