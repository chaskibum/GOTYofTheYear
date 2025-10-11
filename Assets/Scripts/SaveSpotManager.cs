using UnityEngine;

public class SaveSpotManager : MonoBehaviour
{
    private Vector3 _lastSavedRespawnPosition;

    [SerializeField] private bool isMainRespawn;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        _lastSavedRespawnPosition = transform.position;
        if (isMainRespawn)
            GameManager.Instance.GetPlayer.SetMainRespawnPosition(_lastSavedRespawnPosition);
        
        GameManager.Instance.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
        AudioManager.Instance.PlayClip(AudioManager.AudioList.CheckpointActivated);
    }
}
