using PlayerScripts;
using UnityEngine;

public class SaveSpotManager : MonoBehaviour
{
    private Vector3 _lastSavedRespawnPosition;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        _lastSavedRespawnPosition = transform.position;
        
        PlayerController.Instance.SetRespawnPosition(_lastSavedRespawnPosition);
    }
}
