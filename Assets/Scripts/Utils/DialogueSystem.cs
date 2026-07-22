using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameplayElements;
using Managers;
using PlayerScripts;
using ScriptableObjects.Dialogues;
using TMPEffects.Components;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Utils
{
    public class DialogueSystem : MonoBehaviour, IInteractable
    {
        private bool _isInRange;
        private bool _showInteractPrompt = true;
        private bool _dialogueStarted;
        private int _dialogueIndex;
        private const float DialogueSpeed = 0.0165f;
        private bool _canSpeak = true;
        private bool _blockDialogue = false;
        private bool _lastDialogue;
        private int _goodEndingItems = 0;
        private bool _isSpanish = true;
        private List<String> _selectedDialogues = new List<String>();
        private bool _isTyping;
        private Coroutine _writingSound;
        private TMPWriter _writer;

        [Header("DialoguePanel")]
        [SerializeField] private GameObject interactPrompt;
        private TextMeshProUGUI _interactPromptText;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private Transform panelImage;
        [SerializeField] private TMP_Text dialogueText;
        private Vector3 _smallPanel = new Vector3(0.7f, 0.3f, 1);
        private Vector3 _mediumPanel = new Vector3(0.7f, 0.6f, 1);
        private Vector3 _bigPanel = new Vector3(0.7f, 1f, 1);
        [Header("ViejaOptions")]
        [SerializeField] private bool disappearAfterDialogue;
        [SerializeField] private float timeToDisappear;
        [SerializeField] private bool hideVieja;
        [SerializeField] private bool repeatLastDialogue;
        [Header("Ending")]
        [SerializeField] private bool endingDialogue;
        [SerializeField] private bool goodEndingDialogue;
        [SerializeField] private ParticleSystem entranceThunder;
        [SerializeField] private AudioSource thunderSound;
        [SerializeField] private GameObject viejaHitbox;
        [SerializeField] private Image fadeToBlack;
        [SerializeField] private GameObject viejaEnding;
        [SerializeField] private GameObject vieja1;
        [SerializeField] private GameObject vieja2;
        [SerializeField] private GameObject vieja3;
        [SerializeField] private GameObject viejaGoodEnding;
        [SerializeField] private GameObject noRecuerdoVieja;
        private int _viejalIndex = 0;

        [Header("Others")]
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
            _writer = dialoguePanel.GetComponentInChildren<TMPWriter>();

            _interactPromptText = interactPrompt.GetComponentInChildren<TextMeshProUGUI>();
            _interactPromptText.text = GetInteractionPrompt();

            GameManager.Instance.GetGameRestarted?.AddListener(ResetDialogues);

            if (LocalizationSettings.SelectedLocale.ToString() != "Spanish (es)")
            {
                _isSpanish = false;
                _selectedDialogues = dialogueEN.dialogues;
            }
            else
            {
                _selectedDialogues = dialogueES.dialogues;
            }

            if (hideVieja) gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            if (name == "Vieja (1)" && _player.GetDashUnlocked) gameObject.SetActive(false);
            if (name == "Vieja (2)" && _player.GetDoubleJumpUnlocked) gameObject.SetActive(false);
        }

        public string GetInteractionPrompt()
        {
            if (_isSpanish) return "Presiona [E] para hablar";
            else return "Press [E] to talk";
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= LanguageChanged;
        }

        void Update()
        {
            if (_isInRange)
            {
                if (_showInteractPrompt) interactPrompt.SetActive(true);
                else interactPrompt.SetActive(false);
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

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (_isInRange)
            {
                if (_canSpeak)
                {
                    if (ctx.performed) Interact();
                    CalculateDirection();
                }
            }
        }

        private void ExitDialogue()
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

        private void DeactivateDialog()
        {
            _dialogueStarted = false;
            dialoguePanel.SetActive(false);
            _player.inDialog = false;

            if (endingDialogue)
            {
                // ActivateCollider();

                if (vieja1) vieja1.gameObject.SetActive(true);
                if (vieja2) vieja2.gameObject.SetActive(true);
                if (vieja3) vieja3.gameObject.SetActive(true);

                /*if (name == "Vieja2")
                {
                    if (PlayerPrefs.GetString("Viejal1") == "Viejal1" && PlayerPrefs.GetString("Viejal3") != "Viejal3")
                    {
                        viejaGoodEnding.gameObject.SetActive(true);
                        // ActivateCollider();
                        Destroy(vieja2);
                        return;
                    }
                }
                if (name == "Vieja3")
                {
                    if (PlayerPrefs.GetString("Viejal1") == "Viejal1" || PlayerPrefs.GetString("Viejal2") == "Viejal2")
                    {
                        viejaGoodEnding.gameObject.SetActive(true);
                        // ActivateCollider();
                        Destroy(vieja3);
                        return;
                    }
                    else
                    {
                        viejaEnding.GetComponent<BoxCollider2D>().enabled = true;
                    }
                }*/

                if (!vieja1 && !vieja2 && vieja3.transform.localPosition.x == 548.82f)
                {
                    print(vieja3.transform.localPosition.x);
                    print("spawneando vieja buena");
                    viejaGoodEnding.gameObject.SetActive(true);
                    Destroy(vieja3);
                    return;
                }

                /*else if (vieja1.transform.localPosition.x != 548.82f || vieja2.transform.localPosition.x != 548.82f ||
                         vieja3.transform.localPosition.x != 548.82f)
                {
                    viejaEnding.GetComponent<BoxCollider2D>().enabled = true;
                    viejaEnding.GetComponent<SpriteRenderer>().enabled = true;
                }*/

                if (PlayerPrefs.GetString("Viejal1") == "Viejal1" && vieja1 && vieja1.transform.localPosition.x != 548.82f)
                {
                    // DeactivateCollider();
                    viejaEnding.GetComponent<BoxCollider2D>().enabled = false;
                    viejaEnding.GetComponent<SpriteRenderer>().enabled = false;
                    if (noRecuerdoVieja.activeInHierarchy) noRecuerdoVieja.gameObject.SetActive(false);

                    TraerVieja(vieja1);
                    return;
                }
                if (PlayerPrefs.GetString("Viejal2") == "Viejal2" && vieja2 && vieja2.transform.localPosition.x != 548.82f)
                {
                    DeactivateCollider();
                    viejaEnding.GetComponent<BoxCollider2D>().enabled = false;
                    viejaEnding.GetComponent<SpriteRenderer>().enabled = false;
                    if (noRecuerdoVieja.activeInHierarchy) noRecuerdoVieja.gameObject.SetActive(false);

                    // Destroy(vieja1);
                    TraerVieja(vieja2);
                    return;
                }
                if (PlayerPrefs.GetString("Viejal3") == "Viejal3" && vieja3.transform.localPosition.x != 548.82f)
                {
                    DeactivateCollider();
                    viejaEnding.GetComponent<BoxCollider2D>().enabled = false;
                    viejaEnding.GetComponent<SpriteRenderer>().enabled = false;
                    if (noRecuerdoVieja.activeInHierarchy) noRecuerdoVieja.gameObject.SetActive(false);

                    // if (vieja1) Destroy(vieja1);
                    // Destroy(vieja2);
                    TraerVieja(vieja3);
                    return;
                }

                if (vieja1 && vieja1.transform.localPosition.x != 548.82f || vieja2 && vieja2.transform.localPosition.x != 548.82f ||
                    vieja3 && vieja3.transform.localPosition.x != 548.82f)
                {
                    if (viejaEnding.GetComponent<SpriteRenderer>().enabled) return;
                    noRecuerdoVieja.SetActive(true);
                    print("no recuerdo vieja aparece");
                    // viejaEnding.GetComponent<BoxCollider2D>().enabled = true;
                    // viejaEnding.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }

        private void TraerVieja(GameObject vieja)
        {
            var pos = vieja.transform.localPosition;
            pos.x -= 30;
            vieja.transform.localPosition = pos;
        }

        private void EndGame()
        {
            fadeToBlack.DOFade(1, 5f);
            Invoke(nameof(BackToMenu), 5.5f);
        }

        private void BackToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private void ResetDialogues()
        {
            DeactivateDialog();
            gameObject.SetActive(true);
            _sprite.color = new Color(1, 1, 1, 1);
            _canSpeak = true;
            if (hideVieja) gameObject.SetActive(false);
        }

        private IEnumerator Disappear()
        {
            yield return new WaitForSeconds(timeToDisappear);
            DeactivateDialog();
            _showInteractPrompt = false;
            while (_sprite.color.a > 0)
            {
                var color = _sprite.color;
                color.a = color.a - 0.01f;
                _sprite.color = color;
                yield return new WaitForEndOfFrame();
            }
            gameObject.SetActive(false);
        }

        private void ActivateCollider()
        {
            viejaHitbox.SetActive(true);
        }

        private void DeactivateCollider()
        {
            viejaHitbox.SetActive(false);
        }

        private void CanSpeakAgain()
        {
            if (_blockDialogue) return;
            _showInteractPrompt = true;
            interactPrompt.SetActive(true);
            _canSpeak = true;
        }

        private void CalculateDirection()
        {
            _playerToTheRight = _player.GetPlayerPosition.x > transform.position.x;
            _sprite.flipX = _playerToTheRight ? false : true;
        }

        public void Interact()
        {
            if (_isTyping)
            {
                CompleteDialogue();
                return;
            }
            if (!_dialogueStarted) StartDialogue();
            else NextLine();
        }

        private void StartDialogue()
        {
            _dialogueStarted = true;
            GameManager.Instance.GetPlayer.inDialog = true;
            _dialogueIndex = !_lastDialogue ? 0 : _selectedDialogues.Count - 1;
            _player.inDialog = true;
            _showInteractPrompt = false;
            dialoguePanel.SetActive(true);
            StartCoroutine(WriteDialogue());
        }

        private IEnumerator WriteDialogue()
        {
            _isTyping = true;

            dialogueText.text = string.Empty;
            dialogueText.text = _selectedDialogues[_dialogueIndex];
            dialogueText.textWrappingMode = TextWrappingModes.Normal;

            if (dialogueText.text.Length < 50) panelImage.localScale = _smallPanel;
            else if (dialogueText.text.Length < 120) panelImage.localScale = _mediumPanel;
            else panelImage.localScale = _bigPanel;

            foreach (char ch in _selectedDialogues[_dialogueIndex])
            {
                yield return new WaitForSecondsRealtime(DialogueSpeed);

                if (!_isTyping) yield break;
            }

            _isTyping = false;
            _writingSound = null;
        }

        private void NextLine()
        {
            if (name == "ViejaEvil" && _dialogueIndex == _selectedDialogues.Count - 2)
            {
                entranceThunder.Play();
                thunderSound.Play();
                entranceThunder.transform.parent.GetComponent<PolygonCollider2D>().enabled = true;
                Invoke(nameof(ShakeCamera), 0.6f);
            }

            if (name == "ViejaGood" && _dialogueIndex == _selectedDialogues.Count - 7)
            {
                Invoke(nameof(ChoiceDialogue), 0.1f);
            }
            if (_writingSound != null)
            {
                StopCoroutine(_writingSound);
                _writingSound = null;
            }
            if (_dialogueIndex < _selectedDialogues.Count - 1)
            {
                _dialogueIndex++;
                dialogueText.text = _selectedDialogues[_dialogueIndex];
                _writingSound = StartCoroutine(WriteDialogue());
            }
            else
            {
                if (name == "ViejaBadEnding")
                {
                    ActivateCollider();
                }

                if (name == "Vieja1") Destroy(vieja1);
                if (name == "Vieja2") Destroy(vieja2);
                if (name == "Vieja3") Destroy(vieja3);

                if (goodEndingDialogue)
                {
                    _canSpeak = false;
                    GameManager.Instance.GetPlayer.StopInputs();
                    Invoke(nameof(EndGame), 3f);
                    return;
                }
                ExitDialogue();
            }
        }

        private void ChoiceDialogue()
        {
            CompleteDialogue();
            ActivateCollider();
            dialogueText.textWrappingMode = TextWrappingModes.NoWrap;
            panelImage.localScale = new Vector3(1, 1, 1);
        }

        private void ShakeCamera()
        {
            Camera.main.DOShakePosition(0.5f, 2f);
        }

        private void CompleteDialogue()
        {
            _isTyping = false;
            _writer.SkipWriter();

            if (_writingSound != null)
            {
                StopCoroutine(_writingSound);
                _writingSound = null;
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
