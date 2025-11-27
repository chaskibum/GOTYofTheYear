using System.Collections;
using System.Collections.Generic;
using GameplayElements;
using Managers;
using PlayerScripts;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
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

        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField, TextArea(3, 5)] private List<string> dialogueList;
        [SerializeField] private bool disappearAfterDialogue;
        [SerializeField] private float timeToDisappear;
        [SerializeField] private bool hideVieja;
        [SerializeField] private bool endingDialogue;
        [SerializeField] private SpriteRenderer fadeToBlack;
        [SerializeField] private SkillCollectable dash;
    
        [Header("Flip")]
        private bool _playerToTheRight;
        private PlayerController _player;
        private SpriteRenderer _sprite;

        private IEnumerator Start()
        {
            _player = GameManager.Instance.GetPlayer;
            _sprite = GetComponent<SpriteRenderer>();
            
            if (hideVieja) gameObject.SetActive(false);
            GameManager.Instance.GetGameRestarted?.AddListener(ResetDialogues);
            
            yield return new WaitForSeconds(0.1f);
            if (name == "Vieja (1)" && _player.GetDashUnlocked) gameObject.SetActive(false);
            if (name == "Vieja (2)" && _player.GetDoubleJumpUnlocked) gameObject.SetActive(false);
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

        private void CheckInteractInput()
        {
            if (Input.GetButtonDown("Interact"))
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
                if (!disappearAfterDialogue)
                    Invoke(nameof(CanSpeakAgain), 0.7f);
                else
                {
                    if (!_player.GetDashUnlocked)
                        dash.Activate(true);
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
            /*if (disappearAfterDialogue)
            {
                gameObject.SetActive(true);
                _canSpeak = true;
            }*/
            if (hideVieja) gameObject.SetActive(false);
        }

        private void Disappear()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator EndGame()
        {
            while (fadeToBlack.color.a < 1)
            {
                fadeToBlack.color = new Color(0, 0, 0, fadeToBlack.color.a + Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            print("fade finished!");
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
            else if (dialogueText.text == dialogueList[_dialogueIndex]) NextLine();
            else
            {
                StopAllCoroutines();
                dialogueText.text = string.Empty;
                dialogueText.text = dialogueList[_dialogueIndex];
            }
        }

        private void StartDialogue()
        {
            _dialogueStarted = true;
            GameManager.Instance.GetPlayer.inDialog = true;
            _dialogueIndex = 0;
            _player.inDialog = true;
            interactPrompt.SetActive(false);
            dialoguePanel.SetActive(true);
            StartCoroutine(WriteDialogue());
        }

        private IEnumerator WriteDialogue()
        {
            dialogueText.text = string.Empty;
            foreach (char ch in dialogueList[_dialogueIndex])
            {
                // AudioManager.Instance.PlayClip(AudioManager.AudioList.Dialog, true, 0.8f);
                AudioManager.Instance.PlayDialogSound();
                dialogueText.text += ch;
                yield return new WaitForSecondsRealtime(DialogueSpeed);
            }
        }

        private void NextLine()
        {
            if (_dialogueIndex < dialogueList.Count - 1)
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
