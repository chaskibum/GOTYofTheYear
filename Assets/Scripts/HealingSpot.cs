using UnityEngine;

public class HealingSpot : MonoBehaviour, IInteractable
{
    private Animator _animator;
    private bool _isInRange;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        ResetAnimations();
        GameManager.Instance.GetGameRestarted?.AddListener(ResetAnimations);
        GameManager.Instance.GetPlayerRespawn?.AddListener(ResetAnimations);
        GameManager.Instance.GetPlayer.GetPlayerRevived?.AddListener(ResetAnimations);
    }
    
    
    private void OnTriggerStay2D(Collider2D other)
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
            _animator.SetBool("OnRange", true);
        }
        else
        {
            _animator.SetBool("OnRange", false);
        }
    }

    private void CheckInteractInput()
    {
        // if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        if (Input.GetButtonDown("Interact"))
        {
            Interact();
        }
    }

    public void Interact()
    {
        GameManager.Instance.GetPlayer.Heal();
        _animator.SetBool("Used", true);
    }
    
    private void ResetAnimations()
    {
        if (!_animator) return;
        
        _animator.SetBool("Used", false);
    }
}
