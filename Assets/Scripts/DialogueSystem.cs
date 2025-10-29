using System;
using System.Collections;
using System.Collections.Generic;
using PlayerScripts;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour, IInteractable
{
    private bool _isInRange;
    private bool _dialogueStarted;
    private int _dialogueIndex;
    private const float DialogueSpeed = 0.05f;

    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(3, 5)] private List<string> dialogueList;
    
    [Header("Flip")]
    private bool _playerToTheRight;
    private PlayerController _player;
    private SpriteRenderer _sprite;

    private void Start()
    {
        _player = GameManager.Instance.GetPlayer;
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_isInRange)
        {
            CheckInteractInput();
            interactPrompt.SetActive(true);
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }

    private void CheckInteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            Interact();
            CalculateDirection();
        }
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
        interactPrompt.SetActive(false);
        dialoguePanel.SetActive(true);
        StartCoroutine(WriteDialogue());
    }

    private IEnumerator WriteDialogue()
    {
        dialogueText.text = string.Empty;
        foreach (char ch in dialogueList[_dialogueIndex])
        {
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
            _dialogueStarted = false;
            GameManager.Instance.GetPlayer.inDialog = false;
            interactPrompt.SetActive(true);
            dialoguePanel.SetActive(false);
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
