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
    public class HealingSpot : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioSource healSound;
        [SerializeField] private GameObject interactPrompt;
        private TextMeshProUGUI _interactPromptText;

        private Animator _animator;
        private bool _isInRange;
        private bool _isSpanish;
        
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            ResetAnimations();
            AddListeners(true);

            _playerInput.enabled = true;
            
            LocalizationSettings.SelectedLocaleChanged += LanguageChanged;
            LanguageChanged(LocalizationSettings.SelectedLocale);

            _interactPromptText = interactPrompt.GetComponentInChildren<TextMeshProUGUI>();
            _interactPromptText.text = GetInteractionPrompt();
        }

        public string GetInteractionPrompt()
        {
            if (_isSpanish)
            {
                if (GameManager.Instance.GetControllerConnected)
                {
                    return "Presiona <color=yellow>(Y)</color> para curarte";
                }
                else
                {
                    return "Presiona [E] para curarte";
                }
            }
            else
            {
                if (GameManager.Instance.GetControllerConnected)
                {
                    return "Press <color=yellow>(Y)</color> to heal";
                }
                else
                {
                    return "Press [E] to heal";
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

        private void OnDestroy()
        {
            AddListeners(false);
            LocalizationSettings.SelectedLocaleChanged -= LanguageChanged;
        }

        private void AddListeners(bool add)
        {
            if (add)
            {
                GameManager.Instance.GetGameRestarted?.AddListener(ResetAnimations);
                GameManager.Instance.GetPlayerRespawn?.AddListener(ResetAnimations);
                GameManager.Instance.GetPlayer.GetPlayerRevived?.AddListener(ResetAnimations);
            }
            else
            {
                GameManager.Instance.GetGameRestarted?.RemoveListener(ResetAnimations);
                GameManager.Instance.GetPlayerRespawn?.RemoveListener(ResetAnimations);
                GameManager.Instance.GetPlayer.GetPlayerRevived?.RemoveListener(ResetAnimations);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _isInRange = true;
            _interactPromptText.text = GetInteractionPrompt();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _isInRange = false;
        }

        void Update()
        {
            if (_isInRange)
            {
                // CheckInteractInput();
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
            if (_animator.GetBool("Used")) return;
            healSound.Play();
            GameManager.Instance.GetPlayer.Heal();
            _animator.SetBool("Used", true);
        }

        private void ResetAnimations()
        {
            if (!_animator) return;

            _animator.SetBool("Used", false);
        }
    }
}
