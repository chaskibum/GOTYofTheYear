using UnityEngine;

public class ResetObjectPosition : MonoBehaviour
{
    private Vector3 _originalPosition;
    [SerializeField] private SpriteRenderer sprite;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _originalPosition = transform.position;
        GameManager.Instance.GetGameRestarted?.AddListener(ResetPosition);
        GameManager.Instance.GetPlayerRespawn?.AddListener(ResetPosition);
        GameManager.Instance.GetPlayer.GetPlayerRevived?.AddListener(ResetPosition);
        GameManager.Instance.GetResetObjects?.AddListener(ResetPosition);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ResetPosition();
        sprite.transform.eulerAngles += new Vector3(0, 0, 0.5f);
    }

    private void ResetPosition()
    {
        transform.position = _originalPosition;
    }
}
