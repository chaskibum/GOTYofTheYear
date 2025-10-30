using System;
using UnityEngine;

public class SaveSpotManager : MonoBehaviour, IInteractable
{
    private Vector3 _lastSavedRespawnPosition;
    private Animator _animator;
    private static event Action<SaveSpotManager> OnCollisionEvent;

    [SerializeField] private bool isMainRespawn;
    [SerializeField] private AudioSource activatingSound;
    [SerializeField] private GameObject interactPrompt;
    
    private bool _isInRange;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.GetGameRestarted?.AddListener(ResetAnimations);
    }

    private void OnEnable()
    {
        OnCollisionEvent += HandleAnimations;
    }

    private void OnDisable()
    {
        OnCollisionEvent -= HandleAnimations;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _isInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _isInRange = false;
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
        AudioManager.Instance.PlayClip(AudioManager.AudioList.MainCheckpointActivated);
        GameManager.Instance.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
        _animator.SetBool("MainRespawnActivated", true);
        Destroy(interactPrompt);
    }

    private void ActivateBonfireRespawn()
    {
        _lastSavedRespawnPosition = transform.position;
        GameManager.Instance.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
        interactPrompt.SetActive(false);
    }

    private void HandleAnimations(SaveSpotManager activated)
    {
        if (!_animator) return;
        
        bool isActive = activated == this;

        _animator.SetBool("Activated", isActive);
        // interactPrompt.SetActive(!isActive);
    }

    private void ResetAnimations()
    {
        if (!_animator) return;
        
        _animator.SetBool("Activated", false);
    }

    public void PlayActivatingSound()
    {
        activatingSound.Play();
    }
}
