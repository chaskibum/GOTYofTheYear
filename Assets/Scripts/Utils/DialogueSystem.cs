using System;
using System.Collections;
using System.Collections.Generic;
using GameplayElements;
using Managers;
using PlayerScripts;
using ScriptableObjects.Dialogues;
using TMPro;
using UnityEditor.Localization.Editor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class DialogueSystem : MonoBehaviour, IInteractable
    {
        private bool _isInRange;
        private bool _dialogueStarted;
        private int _dialogueIndex;
        private const float DialogueSpeed = 0.05f;
        private bool _canSpeak = true;
        private bool _lastDialogue;
        private bool _isSpanish = true;
        private List<String> _selectedDialogues = new List<String>();

        [Header("DialoguePanel")]
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text dialogueText;
        [Header("ViejaOptions")]
        [SerializeField] private bool disappearAfterDialogue;
        [SerializeField] private float timeToDisappear;
        [SerializeField] private bool hideVieja;
        [SerializeField] private bool repeatLastDialogue;
        [Header("Ending")]
        [SerializeField] private bool endingDialogue;
        [SerializeField] private SpriteRenderer fadeToBlack;
        
        [SerializeField] private SkillCollectable dash;

        [SerializeField] private DialoguesData dialogueES;
        [SerializeField] private DialoguesData dialogueEN;
        
        [Header("Flip")]
        private bool _playerToTheRight;
        private PlayerController _player;
        private SpriteRenderer _sprite;

        private IEnumerator Start()
        {
            LocalizationSettings.SelectedLocaleChanged += LanguageChanged;
            
            _player = GameManager.Instance.GetPlayer;
            _sprite = GetComponent<SpriteRenderer>();
            
            if (hideVieja) gameObject.SetActive(false);
            GameManager.Instance.GetGameRestarted?.AddListener(ResetDialogues);
            
            yield return new WaitForSeconds(0.1f);
            if (name == "Vieja (1)" && _player.GetDashUnlocked) gameObject.SetActive(false);
            if (name == "Vieja (2)" && _player.GetDoubleJumpUnlocked) gameObject.SetActive(false);

            if (LocalizationSettings.SelectedLocale.ToString() != "Spanish (es)")
            {
                _isSpanish = false;
                _selectedDialogues = dialogueEN.dialogues;
            }
            else
            {
                _selectedDialogues = dialogueES.dialogues;
            }
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= LanguageChanged;
        }

        void Update()
        {
            if (_isInRange)
            {
                if (_canSpeak) CheckInteractInput();
                interactPrompt.SetActive(true);
            }
            else
            {
                interactPrompt.SetActive(false);
            }
        }

        private void LanguageChanged(Locale lang)
        {
            if (lang.ToString() != "Spanish (es)")
            {
                _isSpanish = false;
                _selectedDialogues = dialogueEN.dialogues;
            }
            else
            {
                _isSpanish = true;
                _selectedDialogues = dialogueES.dialogues;
            }
        }

        private void CheckInteractInput()
        {
            if (Input.GetButtonDown("GameInteract"))
            {
                Interact();
                CalculateDirection();
            }
        }

        private void ExitDialogue()
        {
            if (endingDialogue) StartCoroutine(EndGame());
            else
            {
                DeactivateDialog();
                _canSpeak = false;
                if (repeatLastDialogue) _lastDialogue = true;
                if (!disappearAfterDialogue)
                    Invoke(nameof(CanSpeakAgain), 0.7f);
                else
                {
                    if (!_player.GetDashUnlocked)
                        dash.Activate(true);
                    else
                    {
                        Invoke(nameof(CanSpeakAgain), 0.7f);
                        StartCoroutine(nameof(Disappear));
                    }
                }
            }
        }

        private void DeactivateDialog()
        {
            _dialogueStarted = false;
            interactPrompt.SetActive(true);
            dialoguePanel.SetActive(false);
            _player.inDialog = false;
        }

        private void ResetDialogues()
        {
            DeactivateDialog();
            gameObject.SetActive(true);
            _canSpeak = true;
            if (hideVieja) gameObject.SetActive(false);
        }

        private IEnumerator Disappear()
        {
            yield return new WaitForSeconds(timeToDisappear);
            DeactivateDialog();
            interactPrompt.SetActive(false);
            AudioManager.Instance.StopClip();
            while (_sprite.color.a > 0)
            {
                var color = _sprite.color;
                color.a = color.a - 0.01f;
                _sprite.color = color;
                yield return new WaitForEndOfFrame();
            }
            gameObject.SetActive(false);
        }

        private IEnumerator EndGame()
        {
            while (fadeToBlack.color.a < 1)
            {
                fadeToBlack.color = new Color(0, 0, 0, fadeToBlack.color.a + Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            
            SceneManager.LoadScene("MainMenu");
        }
    
        private void CanSpeakAgain()
        {
            _canSpeak = true;
        }
    
        private void CalculateDirection()
        {
            _playerToTheRight = _player.GetPlayerPosition.x > transform.position.x;
            _sprite.flipX = _playerToTheRight ? false : true;
        }

        public void Interact()
        {
            if (!_dialogueStarted) StartDialogue();
            // dialogueScriptable
            else if (dialogueText.text == _selectedDialogues[_dialogueIndex]) NextLine();
            else
            {
                StopAllCoroutines();
                dialogueText.text = string.Empty;
                dialogueText.text = _selectedDialogues[_dialogueIndex];
            }
        }

        private void StartDialogue()
        {
            _dialogueStarted = true;
            GameManager.Instance.GetPlayer.inDialog = true;
            _dialogueIndex = !_lastDialogue ? 0 : _selectedDialogues.Count - 1;
            _player.inDialog = true;
            interactPrompt.SetActive(false);
            dialoguePanel.SetActive(true);
            StartCoroutine(WriteDialogue());
        }

        private IEnumerator WriteDialogue()
        {
            dialogueText.text = string.Empty;
            foreach (char ch in _selectedDialogues[_dialogueIndex])
            {
                AudioManager.Instance.PlayDialogSound();
                dialogueText.text += ch;
                yield return new WaitForSecondsRealtime(DialogueSpeed);
            }
        }

        private void NextLine()
        {
            if (_dialogueIndex < _selectedDialogues.Count - 1)
            {
                _dialogueIndex++;
                StartCoroutine(WriteDialogue());
            }
            else
            {
                ExitDialogue();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _isInRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _isInRange = false;
        }
    }
}
