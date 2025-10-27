using System;
using UnityEngine;

public class SaveSpotManager : MonoBehaviour
{
    private Vector3 _lastSavedRespawnPosition;
    private Animator _animator;
    private static event Action<SaveSpotManager> OnCollisionEvent;

    [SerializeField] private bool isMainRespawn;
    [SerializeField] private AudioSource activatingSound;

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
        _lastSavedRespawnPosition = transform.position;
        if (isMainRespawn)
        {
            GameManager.Instance.GetPlayer.SetMainRespawnPosition(_lastSavedRespawnPosition);
            AudioManager.Instance.PlayClip(AudioManager.AudioList.MainCheckpointActivated);
        }
        else
        {
            OnCollisionEvent?.Invoke(this);
        }
        
        GameManager.Instance.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
    }

    private void HandleAnimations(SaveSpotManager activated)
    {
        if (!_animator) return;
        
        bool isActive = activated == this;

        _animator.SetBool("Activated", isActive);
    }

    private void ResetAnimations()
    {
        if (!_animator) return;
        
        _animator.SetBool("Activated", false);
    }

    public void PlayActivatingSound()
    {
        activatingSound.Play();
        print("Activating!!");
    }
}
