using UnityEngine;

public class SaveSpotManager : MonoBehaviour
{
    private Vector3 _lastSavedRespawnPosition;

    [SerializeField] private AudioSource activated;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        _lastSavedRespawnPosition = transform.position;
        
        GameManager.Instance.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
        AudioManager.Instance.PlayClip(AudioManager.AudioList.CheckpointActivated);
    }
}
